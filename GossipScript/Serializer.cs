using System;
using System.IO;
using TranspileTest.Nodes;

namespace TranspileTest
{
    public static class Serializer
    {
        private static readonly int GSBF = 1178751815;
        
        private static byte[] idBuffer = new byte[16];
        public static ScriptNode Read(byte[] bytes)
        {
            ScriptNode node = null;
            using (var ms = new MemoryStream(bytes))
            {
                using (var br = new BinaryReader(ms))
                {
                    // Header
                    var header = br.ReadInt32();    // Magic header
                    if (header != GSBF)
                    {
                        return null;
                    }
                    var versionMajor = br.ReadByte();      // version major
                    var versionMinor = br.ReadByte();      // version major
                    var reservedByte0 = br.ReadByte();
                    var reservedByte1 = br.ReadByte();

                    // Script Node
                    var nodeType = br.ReadUInt16();
                    var commandType = br.ReadUInt16();
                    var childrenCount = br.ReadInt16();

                    var id = br.Read(idBuffer, 0, 16);
                    var guid = new Guid(idBuffer);

                    node = (ScriptNode) ConstuctNode(br, guid, (NodeType) nodeType, (CommandType) commandType);

                    // Read first node
                    for (int i = 0; i < childrenCount; i++)
                    {
                        ReadRecursive(node, br);
                    }
                }
            }
            return node;
        }

        private static Node ConstuctNode(BinaryReader br, Guid id, NodeType nodeType, CommandType commandType)
        {
            //Console.WriteLine(nodeType);
            switch(nodeType)
            {
                case NodeType.Script:
                    {
                        var scriptNode = new ScriptNode();
                        scriptNode.Id = id;
                        scriptNode.readData(br);
                        return scriptNode;
                    }
                case NodeType.Page:
                    {
                        var pageNode = new PageNode();
                        pageNode.Id = id;
                        pageNode.readData(br);
                        return pageNode;
                    }
                case NodeType.OnceOnly:
                    {
                        var onceOnly = new OnceOnlyNode();
                        onceOnly.Id = id;
                        onceOnly.readData(br);
                        return onceOnly;
                    }
                case NodeType.ConditionalTrue:
                    {
                        var node = new ConditionalTrueNode();
                        node.Id = id;
                        node.readData(br);
                        return node;
                    }
                case NodeType.ConditionalFalse:
                    {
                        var node = new ConditionalFalseNode();
                        node.Id = id;
                        node.readData(br);
                        return node;
                    }
                case NodeType.OptionsChoice:
                    {
                        var node = new ShowOptionsNode();
                        node.Id = id;
                        node.readData(br);
                        return node;
                    }
                case NodeType.Option:
                    {
                        var node = new OptionNode();
                        node.Id = id;
                        node.readData(br);
                        return node;
                    }
                case NodeType.ParallelNode:
                    {
                        var node = new ParallelNode();
                        node.Id = id;
                        node.readData(br);
                        return node;
                    }
                case NodeType.BlockNode:
                    {
                        var node = new BlockNode();
                        node.Id = id;
                        node.readData(br);
                        return node;
                    }
                case NodeType.Command:
                    {
                        switch(commandType)
                        {
                            case CommandType.Say:
                                {
                                    var command = new SayNode();
                                    command.Id = id;
                                    command.readData(br);
                                    return command;
                                }
                            case CommandType.CallPage:
                                {
                                    var command = new CallPageNode();
                                    command.Id = id;
                                    command.readData(br);
                                    return command;
                                }
                            case CommandType.Return:
                                {
                                    var command = new ReturnNode();
                                    command.Id = id;
                                    command.readData(br);
                                    return command;
                                }
                            case CommandType.Wait:
                                {
                                    var command = new WaitNode();
                                    command.Id = id;
                                    command.readData(br);
                                    return command;
                                }
                            case CommandType.Print:
                                {
                                    var command = new PrintNode();
                                    command.Id = id;
                                    command.readData(br);
                                    return command;
                                }
                            default:
                                throw new Exception("Unhandled Node Type:" + nodeType + ":" + commandType);
                        }
                    }
               // case NodeType.Say
                default:
                    throw new Exception("Unhandled Node Type:" + nodeType);
            }
            return null;
        }

        private static void ReadRecursive(Node root, BinaryReader br)
        {
            var nodeType = br.ReadUInt16();
            var commandType = br.ReadUInt16();
            var childrenCount = br.ReadInt16();

            var id = br.Read(idBuffer, 0, 16);
            var guid = new Guid(idBuffer);

            var node = ConstuctNode(br, guid, (NodeType) nodeType, (CommandType) commandType);
            root.AddChildNode(node);
            
            for (int i = 0; i < childrenCount; i++)
            {
                ReadRecursive(node, br);
            }
        }

        public static byte[] Write(Node root)
        {
            byte[] result = null;
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write((uint)GSBF);   // Magic Header
                    bw.Write((byte)1);      // Version Major
                    bw.Write((byte)1);      // Version Minor
                    bw.Write((byte)0);      // Reserved Byte
                    bw.Write((byte)0);      // Reserved Byte

                    WriteRecursive(root, bw);

                    result = ms.ToArray();
                }
            }
            return result;
        }

        public static void WriteRecursive(Node root, BinaryWriter bw)
        {
            bw.Write((ushort)root.NodeType);
            bw.Write((ushort)root.CommandType);
            bw.Write((ushort)root.Children.Count);
            bw.Write(root.Id.ToByteArray());
            // TODO Write parameter count, write keys and values
            root.writeData(bw);
            foreach (var n in root.Children)
            {
                WriteRecursive(n, bw);
            }
        }
    }
}
