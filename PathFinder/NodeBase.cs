using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BasDidon.PathFinder.NodeBase
{
    public interface INodeRegistry
    {
        public IReadOnlyList<INode> Nodes { get; }
        public IReadOnlyList<INodeEdge> NodeEdges { get; }
    }

    public interface INode
    {
        public INode[] NextNodes { get; }
    }

    public interface INodeEdge
    {
        public INode From { get; }
        public INode To { get; }
        public bool IsOneWay { get; } 
        public bool IsActive { get; }
    }

    public class NodePath
    {
        List<INode> nodes;
        public IReadOnlyList<INode> Nodes => nodes;

        public NodePath()
        {
            nodes = new();
        }

        public NodePath(INode StartNode) : base()
        {
            nodes.Add(StartNode);
        }

        public NodePath(IEnumerable<INode> nodes)
        {
            this.nodes = new(nodes);
        }

        public NodePath(IEnumerable<INode> nodes, INode node)
        {
            this.nodes = new(nodes) { node };
        }

        public INode LastNode => nodes.Last();
    }

    public class PathFinder
    {
        public IEnumerable<NodePath> FindPathByMove(INode startNode,int move)
        {
            if (move <= 0)
                return null;

            List<NodePath> nodePaths = new();
            List<NodePath> toSearch = new();

            for(int i = 0; i < move; i++)
            {
                if(i == 0)
                {
                    foreach(var nextNode in startNode.NextNodes)
                    {
                        nodePaths.Add(new(nextNode));
                    }
                }
                else
                {
                    toSearch = nodePaths.ToList();
                    nodePaths = toSearch.SelectMany(p => p.LastNode.NextNodes.Select(n => new NodePath(p.Nodes, n))).ToList();                    
                }
            }

            return nodePaths;
        }
    }
}
