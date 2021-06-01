using ExpressionParser;
using System;
using System.Collections.Generic;
using System.Threading;
using TranspileTest.Nodes;

namespace TranspileTest
{
    public class NodeStackItem
    {
        public Node Node;

        public Int32 Index;

        public NodeStackItem()
        {

        }

        public NodeStackItem(Node n, Int32 i)
        {
            Node = n;
            Index = i;
        }
    }




    public class ProgramData
    {
        public ScriptVariableTable ScriptVariableTable;

        public float DeltaTime;

        public HashSet<UInt32> AutoRemoveOptionsChecks = new HashSet<UInt32>();

        public HashSet<UInt32> OnceOnlyChecks = new HashSet<UInt32>();

        public ExpressionRunner expressionRunner = new ExpressionRunner();

        public void RemoveOption(UInt32 id)
        {
            lock (AutoRemoveOptionsChecks)
            {
                if (!AutoRemoveOptionsChecks.Contains(id))
                {
                    AutoRemoveOptionsChecks.Add(id);
                }
            }
        }

        public bool HasRemovedOption(UInt32 id)
        {
            return AutoRemoveOptionsChecks.Contains(id);
        }

        internal bool HasRunOnceOnly(UInt32 id)
        {
            return OnceOnlyChecks.Contains(id);
        }

        internal void MarkOnceOnly(UInt32 id)
        {
            lock (OnceOnlyChecks)
            {
                if (!OnceOnlyChecks.Contains(id))
                {
                    OnceOnlyChecks.Add(id);
                }
            }
        }
    }

    public class ProgramCounter
    {
        public Stack<NodeStackItem> NodeStack = new Stack<NodeStackItem>();

        public List<Stack<NodeStackItem>> ParallelStack = new List<Stack<NodeStackItem>>();


        // TODO Move these into another class away from the stack variables above
        // These return registers are a little side affecty, but they should be ok as the data should be
        // used straight away in conjuction with the return result of a given node.
        public string ReturnRegisterString;

        public int ReturnRegisterInt32;
    }

    public enum ScriptResult
    {
        Undefined,
        FinishedSuccessfully,
        FinishedWithError,
        Yielded,
        Running
    }

    public class NodeEngine
    {
        ProgramCounter programIndex = new ProgramCounter();
        ProgramData programData = new ProgramData();

        // Parallel index?

        // TODO Set the variables for the Program
        public void LoadVariableTable(ScriptVariableTable variableTable)
        {
            // TODO initalize with copies instead of reference
            programData.ScriptVariableTable = variableTable;
        }

        public ScriptResult RunScriptContinous(ScriptNode script)
        {
            programIndex.NodeStack.Push(new NodeStackItem(script, 0));

            var result = ScriptResult.Undefined;
            while(result != ScriptResult.FinishedSuccessfully &&
                  result != ScriptResult.FinishedWithError)
            {
                result = RunScript(script);
            }

            return result;
        }
        
        private ScriptResult RunScript(ScriptNode script)
        {
            bool isRunning = true;
            while (isRunning)
            {
                // Update Timing
                // Bit of a hack for now
                {
                    Thread.Sleep(16);
                    programData.DeltaTime = 16;
                }

                
                // Check if finished
                if (programIndex.NodeStack.Count == 0)
                {
                    isRunning = false;
                    continue;
                }

                // Check the parallel node stack has anything in it
                {
                    //if (programIndex.ParallelStack.Count != 0)
                    while(programIndex.ParallelStack.Count != 0)
                    {
                        // TODO Each program data needs it's own delta time
                        var parallelNodes = programIndex.ParallelStack.ToArray();
                        foreach (var nodeStack in parallelNodes)
                        {
                            if (nodeStack.Count == 0)
                            {
                                // done
                                programIndex.ParallelStack.Remove(nodeStack);
                            }
                            else
                            {
                               var n = nodeStack.Peek();
                               var result = RunNode(script, nodeStack, n, programIndex, programData);
                            }
                        }

                        return ScriptResult.Yielded; // Running parallel
                    }
                }

                // Standard Stack
                {
                    var current = programIndex.NodeStack.Peek();
                    var result = RunNode(script, programIndex.NodeStack,current,programIndex, programData);

                    if (result != ScriptResult.Running)
                    {
                        return result;
                    }
                }
            }

            return ScriptResult.FinishedSuccessfully;
        }

        private static ScriptResult RunNode(ScriptNode script, 
                                            Stack<NodeStackItem> nodestack, 
                                            NodeStackItem current,
                                            ProgramCounter programIndex,
                                            ProgramData programData) // TODO Change
        {

            // Only execute container nodes:
            if (current.Node.NodeType == NodeType.Script ||
                current.Node.NodeType == NodeType.Page ||
                current.Node.NodeType == NodeType.OptionsChoice ||
                current.Node.NodeType == NodeType.Option ||
                current.Node.NodeType == NodeType.OnceOnly ||
                current.Node.NodeType == NodeType.ConditionalTrue ||
                current.Node.NodeType == NodeType.ConditionalFalse ||
                current.Node.NodeType == NodeType.BlockNode ||
                current.Node.NodeType == NodeType.ParallelNode ||
                current.Node.NodeType == NodeType.ConditionalIf
                )
            {
                // Run the children
                if (current.Index >= current.Node.Children.Count)
                {
                    nodestack.Pop();
                    return ScriptResult.Running;
                }

                var childNode = current.Node.Children[current.Index];
                if (childNode.NodeType == NodeType.Page)
                {
                    nodestack.Push(new NodeStackItem(current.Node.Children[0], 0));
                    return ScriptResult.Running;
                }

                var returnResult = childNode.Run(ref programIndex, ref programData);
                if (returnResult == NodeRunResult.Await)
                {
                    // return and then re-enter on this node
                    return ScriptResult.Yielded;
                }

                // Result filter
                if (returnResult == NodeRunResult.NextCommand)
                {
                    current.Index += 1;
                    if (current.Index >= current.Node.Children.Count)
                    {
                        LevelUp(nodestack);
                        return ScriptResult.Running;
                    }
                    return ScriptResult.Running;
                }
                else if (returnResult == NodeRunResult.PushPage)
                {
                    nodestack.Push(new NodeStackItem(childNode, current.Index)); // Add a CallNode so we know where to return to.
                    var pageNode = script.FindPageByName(programIndex.ReturnRegisterString);
                    nodestack.Push(new NodeStackItem( pageNode, 0));              // Our new target PageNode
                    return ScriptResult.Running;
                }
                else if (returnResult == NodeRunResult.PopPage)
                {
                    LevelUp(nodestack);
                    return ScriptResult.Running;
                }
                else if (returnResult == NodeRunResult.PushChildN)
                {
                    nodestack.Push(new NodeStackItem(childNode, programIndex.ReturnRegisterInt32)); // Push current node to return to
                    return ScriptResult.Running;
                }
                else if (returnResult == NodeRunResult.PushChildFirst)
                {
                    nodestack.Push(new NodeStackItem(childNode, 0)); // Push current node to return to
                    return ScriptResult.Running;
                }
                else if (returnResult == NodeRunResult.PushChildTrue)
                {
                    // TODO Find childnode that is true rather than assume first
                    nodestack.Push(new NodeStackItem(childNode, 0)); // Push current node to return to
                    return ScriptResult.Running;
                }
                else if (returnResult == NodeRunResult.PushChildFalse)
                {
                    // TODO Find childnode that is false rather than assume second
                    nodestack.Push(new NodeStackItem(childNode, 1));
                    return ScriptResult.Running;
                }
                
                else if (returnResult == NodeRunResult.PushParallel)
                {
                    // Add all block children to a new parallel stack
                    
                    for(int i= childNode.Children.Count-1;i>=0;i--)
                    {
                        var pstack = new Stack<NodeStackItem>(); // TODO From pool

                        var c = childNode.Children[i];
                        pstack.Push(new NodeStackItem(c, 0));
                        programIndex.ParallelStack.Add(pstack);

                    }

                    // Go to the next node when we are ready
                    current.Index += 1;
                    if (current.Index >= current.Node.Children.Count)
                    {
                        LevelUp(nodestack);
                        return ScriptResult.Running;
                    }
                    return ScriptResult.Running;

                }
                else
                {
                    Console.WriteLine("TODO Implement Return Result: " + returnResult);
                    return ScriptResult.Running;
                }
            }
            else
            {
                Console.WriteLine("TODO Implement NodeType: " + current.Node.NodeType);
            }

            return ScriptResult.Running;
        }

        private static void LevelUp(Stack<NodeStackItem> nodestack)
        {
            if (nodestack.Count == 0)
                return;

            var peek = nodestack.Peek();

            // If we are returning to a page or a script
            // We want to move up the call stack
            while (peek.Node.NodeType == NodeType.Page ||
                   peek.Node.NodeType == NodeType.Script ||
                   //peek.Node.NodeType == NodeType.OptionsChoice ||
                   peek.Node.NodeType == NodeType.Option ||
                   peek.Node.NodeType == NodeType.ConditionalFalse ||
                   peek.Node.NodeType == NodeType.ConditionalTrue ||
                   peek.Node.NodeType == NodeType.BlockNode)

                   // Not sure what we want to do about parallel / blocks here...
            {
                nodestack.Pop();
                if (nodestack.Count==0)
                {
                    return;
                }
                peek = nodestack.Peek();
            }
            
            // Move to the next node
            {
                var active = nodestack.Pop();
                if (active.Node.NodeType == NodeType.OptionsChoice)
                {
                    return;
                }

                // Want to move to the next node!
                // Either stay on this level or move up
                var parent = nodestack.Peek();
                if (parent != null)
                {
                    parent.Index += 1;
                    if (parent.Index >= parent.Node.Children.Count)
                    {
                        LevelUp(nodestack);
                        return;
                    }
                    else
                    {
                        // Stay on this level
                    }
                }
            }
        }
    }
}
