using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPilotRabbitHole
{
  class Node
  {
    private char _value;
    private bool _isWord;
    private Node _parent;
    private List<Node> _children;
    private Node n;

    public char Value { get => _value; }
    public bool IsWord { get => _isWord; set => _isWord = value; }
    public Node Parent { get { return _parent; } }
    public List<Node> Children { get { return _children;} }
    
    //public SortedSet<Node> Children;
    public Node(char value, Node parent)
    {
      _value = value;
      _parent = parent;
      _children = new List<Node>();
    }

    public Node FindChild(char c)
    {
      foreach (Node n in _children)
      {
        if (n.Value == c)
        {
          return n;
        }
      }
      return null;
    }


    public Node AddChild(char c)
    {
      Node n = new Node(c, this);
      this._children.Add(n);
      return n;
    }
  }
}
