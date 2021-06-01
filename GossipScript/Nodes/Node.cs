using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspileTest.Nodes
{
    public enum NodeType
    {
        Undefined = 0,
        Page = 1,
        Script = 2,
        Command = 3,
        CustomCommand = 4,
        Option = 5,
        OptionsChoice = 6,
        ConditionalTrue = 7,
        ConditionalFalse = 8,
        ConditionalIf = 9,
        OnceOnly = 10,
        ParallelNode = 11,
        BlockNode = 12
    }

    public enum CommandType
    {
        Undefined = 1,
        Say = 2,
        CallPage = 3,
        Return = 4,
        Wait = 5,
        Print = 6,
        SetVar = 7
    }

    public enum NodeRunResult
    {
        NextCommand = 0,
        PushPage = 1,
        PopPage = 2,
        PopScript = 3,
        EndScriptNaturally = 4,
        Await = 5,
        PushChildN = 6,
        PushChildTrue = 7,
        PushChildFalse = 8,
        PushChildFirst = 9,
        PushParallel = 10
        //Wait
    }

    public class Node
    {
        public UInt32 Id;
        public NodeType NodeType = NodeType.Undefined;
        public CommandType CommandType = CommandType.Undefined;

        public List<Node> Children = new List<Node>();

        public bool HasChildren() { return Children.Count() > 0; }

        // TODO Return result enum
        public virtual NodeRunResult Run(ref ProgramCounter pc, ref ProgramData pd)
        {
            Console.WriteLine("TODO Implement Execute for:" + NodeType + " " + CommandType);
            return NodeRunResult.NextCommand;
        }

        public virtual void writeData(BinaryWriter bw)
        {
            throw new NotImplementedException("Write Data");
        }

        public virtual void readData(BinaryReader br)
        {
            throw new NotImplementedException("Read Data");
        }

        public Node AddChildNode(Node node)
        {
            Children.Add(node);
            return node;
        }

        public Node AddCommandNode(Node node)
        {
            Children.Add(node);
            return this;
        }

        internal Node FindPageByName(string name)
        {
            foreach(var c in Children)
            {
                if (c is PageNode)
                {
                    var pageNode = c as PageNode;
                    var result = FindPageByNameRecursive(pageNode, name);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        private Node FindPageByNameRecursive(PageNode root, String name)
        {
            if (root.PageName == name)
            {
                return root;
            }
            else
            {
                foreach (var c in root.Children)
                {
                    if (c is PageNode)
                    {
                        var pageNode = c as PageNode;
                        FindPageByNameRecursive(pageNode, name);
                    }
                }
            }
            return null;
        }
    }
}
