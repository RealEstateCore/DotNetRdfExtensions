// <copyright file="NodeShape.cs" company="RealEstateCore Consortium">
// Copyright (c) RealEstateCore Consortium. All rights reserved.
// </copyright>

namespace RealEstateCore.DotNetRdfExtensions.SHACL
{
    using VDS.RDF;
    using VDS.RDF.Query.Builder;
    using VDS.RDF.Shacl;

    public class NodeShape : Shape
    {
        public NodeShape(IUriNode node, ShapesGraph graph)
            : base(node, graph)
        {
        }

        public new IUriNode Node
        {
            get
            {
                return (IUriNode)base.Node;
            }
        }

        public Uri Uri => this.Node.Uri;

        public IEnumerable<PropertyShape> PropertyShapes
        {
            get
            {
                IUriNode shProperty = this.Graph.CreateUriNode(SH.property);
                foreach (Triple t in this.Graph.GetTriplesWithSubjectPredicate(this.Node, shProperty))
                {
                    yield return new PropertyShape(t.Object, this.Graph);
                }
            }
        }

        public IEnumerable<NodeShape> DirectSuperShapes
        {
            get
            {
                IUriNode rdfsSubClassOf = this.Graph.CreateUriNode(RDFS.subClassOf);
                foreach (Triple t in this.Graph.GetTriplesWithSubjectPredicate(this.Node, rdfsSubClassOf))
                {
                    if (t.Object is IUriNode superClass && superClass.IsNodeShape())
                    {
                        yield return new NodeShape(superClass, this.Graph);
                    }
                }
            }
        }

        public IEnumerable<NodeShape> SuperShapes
        {
            get
            {
                IEnumerable<NodeShape> directSuperShapes = this.DirectSuperShapes;
                HashSet<NodeShape> allSuperShapes = new HashSet<NodeShape>();
                allSuperShapes.UnionWith(directSuperShapes);
                foreach (NodeShape superClass in directSuperShapes)
                {
                    allSuperShapes.UnionWith(superClass.SuperShapes);
                }

                return allSuperShapes;
            }
        }

        public IEnumerable<NodeShape> DirectSubShapes
        {
            get
            {
                IUriNode rdfsSubClassOf = this.Graph.CreateUriNode(RDFS.subClassOf);
                foreach (Triple t in this.Graph.GetTriplesWithPredicateObject(rdfsSubClassOf, this.Node))
                {
                    if (t.Subject is IUriNode subClassNode && subClassNode.IsNodeShape())
                    {
                        yield return new NodeShape(subClassNode, this.Graph);
                    }
                }
            }
        }

        public IEnumerable<NodeShape> SubShapes
        {
            get
            {
                IEnumerable<NodeShape> directSubShapes = this.DirectSubShapes;
                HashSet<NodeShape> allSubShapes = new HashSet<NodeShape>();
                allSubShapes.UnionWith(directSubShapes);
                foreach (NodeShape subClass in directSubShapes)
                {
                    allSubShapes.UnionWith(subClass.SubShapes);
                }

                return allSubShapes;
            }
        }

        public List<IUriNode> LongestSuperShapesPath
        {
            get
            {
                IEnumerable<NodeShape> directSuperShapes = this.DirectSuperShapes.Where(superShape => !superShape.IsDeprecated);
                if (directSuperShapes.Count() < 1 || directSuperShapes.Any(superClass => superClass.IsTopThing))
                {
                    return new List<IUriNode>();
                }
                else
                {
                    // Assume the first parent has the longest path; if not, it will be replaced in subsequent foreach
                    NodeShape longestParent = directSuperShapes.First();
                    List<IUriNode> longestParentPath = longestParent.LongestSuperShapesPath;

                    // Iterate through the other parents to see if any is longer
                    foreach (NodeShape possibleSuperShape in directSuperShapes.Skip(1))
                    {
                        List<IUriNode> possibleSuperShapeParents = possibleSuperShape.LongestSuperShapesPath;
                        if (possibleSuperShapeParents.Count() > longestParentPath.Count())
                        {
                            longestParent = possibleSuperShape;
                            longestParentPath = possibleSuperShapeParents;
                        }
                    }

                    // At this point longestParentPath + longestParent should together contain the longest path to the root; return them
                    longestParentPath.Add(longestParent.Node);
                    return longestParentPath;
                }
            }
        }

        public bool IsTopThing
        {
            get
            {
                string[] topLevelThings =
                {
                    OWL.Thing.AbsoluteUri,
                    RDFS.Resource.AbsoluteUri,
                };
                return topLevelThings.Contains(this.Node.Uri.AbsoluteUri);
            }
        }

        public void AddSuperClass(NodeShape superShape)
        {
            this.AddSuperClass(superShape.Node);
        }

        public void AddSuperClass(IUriNode superClass)
        {
            IUriNode rdfsSubClassOf = this.Graph.CreateUriNode(RDFS.subClassOf);
            this.Graph.Assert(this.Node, rdfsSubClassOf, superClass);
        }

        public PropertyShape CreatePropertyShape(Uri path)
        {
            IBlankNode pShapeNode = this.Graph.CreateBlankNode();
            IUriNode rdfType = this.Graph.CreateUriNode(RDF.type);
            IUriNode shPropertyShape = this.Graph.CreateUriNode(SH.PropertyShape);
            this.Graph.Assert(pShapeNode, rdfType, shPropertyShape);

            IUriNode shPath = this.Graph.CreateUriNode(SH.path);
            IUriNode pathNode = this.Graph.CreateUriNode(path);
            this.Graph.Assert(pShapeNode, shPath, pathNode);

            IUriNode shProperty = this.Graph.CreateUriNode(SH.property);
            this.Graph.Assert(this.Node, shProperty, pShapeNode);

            return new PropertyShape(pShapeNode, this.Graph);
        }
    }
}