using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BasDidon.PathFinder.NodeBase
{
    public interface INodeRegistry<T> where T:INode<T>
    {
        public IReadOnlyList<INode<T>> Nodes { get; }
        public IReadOnlyList<INodeEdge<T>> NodeEdges { get; }
    }

    public interface INode<T> where T : INode<T>
    {
        public List<T> NextNodes { get; }
    }

    public interface INodeEdge<T> where T:INode<T>
    {
        public T From { get; }
        public T To { get; }
        public bool IsOneWay { get; } 
        public bool IsActive { get; }
    }

    public class NodePath<T> where T:INode<T>
    {
        List<T> nodes;
        public IReadOnlyList<T> Nodes => nodes;

        public NodePath(T StartNode)
        {
            nodes = new() { StartNode};
        }

        public NodePath(IEnumerable<T> nodes)
        {
            this.nodes = new(nodes);
        }

        public NodePath(IEnumerable<T> nodes, T node)
        {
            this.nodes = new(nodes) { node };
        }

        public T Last => nodes.Last();  // Last node can't be null by designed
    }

    public static class PathFinder
    {
        public static IEnumerable<NodePath<T>> FindPathByMove<T>(T startNode, int move) where T : INode<T>
        {
            if (move <= 0)
                return null;

            var nodePaths = new List<NodePath<T>>().AsEnumerable();

            for (int i = 0; i < move; i++)
            {
                nodePaths = i == 0
                    ? startNode.NextNodes.Select(nextNode => new NodePath<T>(nextNode))
                    : nodePaths.SelectMany(p => p.Last.NextNodes.Select(n => new NodePath<T>(p.Nodes, n)));
            }

            return nodePaths;
        }
    }
}
