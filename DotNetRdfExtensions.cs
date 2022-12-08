// <copyright file="DotNetRdfExtensions.cs" company="RealEstateCore Consortium">
// Copyright (c) RealEstateCore Consortium. All rights reserved.
// </copyright>

namespace RealEstateCore.DotNetRdfExtensions
{
    using VDS.RDF;
    using VDS.RDF.Nodes;
    using VDS.RDF.Parsing;

    /// <summary>
    /// A set of hopefully useful extension methods for core DotNetRdf classes.
    /// </summary>
    public static class DotNetRdfExtensions
    {
        /// <summary>
        /// Checks whether a certain triple exists in a graph. Syntactic sugar for <see cref="IGraph.ContainsTriple(Triple)"/>.
        /// </summary>
        /// <param name="graph">The graph to be checked.</param>
        /// <param name="subj">Subject node of triple to check for.</param>
        /// <param name="pred">Predicate node of triple to check for.</param>
        /// <param name="obj">Object node of triple to check for.</param>
        /// <returns><c>true</c> if <paramref name="graph"/> contains the sought triple, else <c>false</c>.</returns>
        public static bool ContainsTriple(this IGraph graph, INode subj, INode pred, INode obj)
        {
            return graph.ContainsTriple(new Triple(subj, pred, obj));
        }

        /// <summary>
        /// Checks whether a given RDF node is an RDFS or OWL class.
        /// </summary>
        /// <param name="node">Node to be checked.</param>
        /// <returns><c>true</c> if the node is an RDFS or OWL class, else <c>false</c>.</returns>
        public static bool IsClass(this INode node)
        {
            return node.IsRdfsClass() || node.IsOwlClass();
        }

        /// <summary>
        /// Checks if a given RDF node is an OWL class.
        /// </summary>
        /// <param name="node">Node to be checked.</param>
        /// <returns><c>true</c> if the node is an OWL class, else <c>false</c>.</returns>
        public static bool IsOwlClass(this INode node)
        {
            IGraph graph = node.Graph;
            IUriNode rdfType = graph.CreateUriNode(RDF.type);
            IUriNode owlClass = graph.CreateUriNode(OWL.Class);
            return graph.ContainsTriple(node, rdfType, owlClass);
        }

        /// <summary>
        /// Checks if a given RDF node is an RDFS class.
        /// </summary>
        /// <param name="node">Node to be checked.</param>
        /// <returns><c>true</c> if the node is an RDFS class, else <c>false</c>.</returns>
        public static bool IsRdfsClass(this INode node)
        {
            IGraph graph = node.Graph;
            IUriNode rdfType = graph.CreateUriNode(RDF.type);
            IUriNode rdfsClass = graph.CreateUriNode(RDFS.Class);
            return graph.ContainsTriple(node, rdfType, rdfsClass);
        }

        /// <summary>
        /// Returns all literal nodes asserted as rdfs:label on a given input node.
        /// </summary>
        /// <param name="node">Node whose labels will be returned.</param>
        /// <returns>All literal nodes asserted to be rdfs:label of the input node.</returns>
        public static IEnumerable<ILiteralNode> RdfsLabels(this INode node)
        {
            IUriNode rdfsLabel = node.Graph.CreateUriNode(RDFS.label);
            foreach (Triple t in node.Graph.GetTriplesWithSubjectPredicate(node, rdfsLabel))
            {
                if (t.Object.NodeType == NodeType.Literal)
                {
                    yield return (ILiteralNode)t.Object;
                }
            }
        }

        /// <summary>
        /// Returns all literal nodes asserted as rdfs:comment on a given input node.
        /// </summary>
        /// <param name="node">Node whose comments will be returned.</param>
        /// <returns>All literal nodes asserted to be rdfs:comment of the input node.</returns>
        public static IEnumerable<ILiteralNode> RdfsComments(this INode node)
        {
            IUriNode rdfsComment = node.Graph.CreateUriNode(RDFS.comment);
            foreach (Triple t in node.Graph.GetTriplesWithSubjectPredicate(node, rdfsComment))
            {
                if (t.Object.NodeType == NodeType.Literal)
                {
                    yield return (ILiteralNode)t.Object;
                }
            }
        }

        /// <summary>
        /// Checks whether a certain node has an owl:deprecated annotation property set on it, and if
        /// that annotation is set as true.
        /// </summary>
        /// <param name="node">Node to check for deprecation.</param>
        /// <returns><c>true</c> if node is deprecated, else <c>false</c>.</returns>
        public static bool IsOwlDeprecated(this INode node)
        {
            IGraph graph = node.Graph;
            IUriNode owlDeprecated = graph.CreateUriNode(OWL.deprecated);
            return graph.GetTriplesWithSubjectPredicate(node, owlDeprecated).Objects().LiteralNodes().Any(deprecationNode => deprecationNode.AsValuedNode().AsBoolean() == true);
        }

        /// <summary>
        /// Returns all literal nodes asserted as skos:definition on a given input node.
        /// </summary>
        /// <param name="node">Node whose definitions will be returned.</param>
        /// <returns>All literal nodes asserted to be skos:definition of the input node.</returns>
        public static IEnumerable<ILiteralNode> SkosDefinitions(this INode node)
        {
            IUriNode skosDefinition = node.Graph.CreateUriNode(SKOS.definition);
            foreach (Triple t in node.Graph.GetTriplesWithSubjectPredicate(node, skosDefinition))
            {
                if (t.Object.NodeType == NodeType.Literal)
                {
                    yield return (ILiteralNode)t.Object;
                }
            }
        }

        /// <summary>
        /// Returns the namespace of this URI node, i.e., the counterpart to <seealso cref="LocalName(IUriNode)"/>.
        /// </summary>
        /// <param name="node">Input URI node.</param>
        /// <returns>
        /// If the URI is hash-based (i.e., if it has an ending prepended by #), then everything up to and including the hash
        /// sign is returned; otherwise, everything up to the last forward slash.
        /// <br/><br/>
        /// I.e.:<br/>
        /// <c>https://w3id.org/rec#Building</c> yields <c>https://w3id.org/rec#</c>.<br />
        /// <c>https://w3id.org/rec/Refrigerator</c> yields <c>https://w3id.org/rec/</c>.
        /// </returns>
        /// <exception cref="UriFormatException">If a namespace/local name separator cannot be found, e.g.,
        /// if the input URI is a URN (<c>urn:isbn:0451450523</c>), or if it lacks a path component
        /// (<c>https://w3id.org/</c>).</exception>
        public static Uri GetNamespace(this IUriNode node)
        {
            if (node.Uri.Fragment.Length > 0)
            {
                return new Uri(node.Uri.GetLeftPart(UriPartial.Path) + "#");
            }

            string nodeUriPath = node.Uri.GetLeftPart(UriPartial.Path);
            if (nodeUriPath.Count(x => x == '/') >= 3)
            {
                string nodeUriBase = nodeUriPath.Substring(0, nodeUriPath.LastIndexOf("/", StringComparison.Ordinal) + 1);
                return new Uri(nodeUriBase);
            }

            throw new UriFormatException($"The Uri {node.Uri} doesn't contain a namespace/local name separator.");
        }

        /// <summary>
        /// Returns the local portion of the URI of this node.
        /// </summary>
        /// <param name="node">Input URI node.</param>
        /// <returns>
        /// If the URI is hash-based (i.e., if it has an ending prepended by #), then the fragment following the hash
        /// sign is returned; otherwise, the .NET method <see cref="Path.GetFileName(string?)"/> is called, typically
        /// returning the portion of the URI that follows the last forward slash.
        /// <br/><br/>
        /// I.e.:<br/>
        /// <c>https://w3id.org/rec#Building</c> yields <c>Building</c>.<br />
        /// <c>https://w3id.org/rec/Refrigerator</c> yields <c>Refrigerator</c>.
        /// </returns>
        public static string LocalName(this IUriNode node)
        {
            if (node.Uri.Fragment.Length > 0)
            {
                return node.Uri.Fragment.Trim('#');
            }

            return Path.GetFileName(node.Uri.AbsolutePath);
        }

        /// <summary>
        /// Checks whether a given node is in the XMLSchema namespace.
        /// </summary>
        /// <param name="node">Node to check.</param>
        /// <returns><c>true</c> if the node's URI starts with <c>http://www.w3.org/2001/XMLSchema#</c>, else <c>false</c>.</returns>
        public static bool IsXsdType(this IUriNode node)
        {
            return node.Uri.AbsoluteUri.StartsWith(XmlSpecsHelper.NamespaceXmlSchema);
        }

        /// <summary>
        /// Returns any URI nodes for which the input URI node is in the rdfs:domain (typically RDF or OWL properties).
        /// </summary>
        /// <param name="node">Input node for which properties are sought.</param>
        /// <returns>All URI nodes that are subjects of triples where the predicate is rdfs:domain and the object is
        /// the input node. E.g., <c>ex:SoughtNode rdfs:domain ex:InputNode</c>.</returns>
        public static IEnumerable<IUriNode> PropertiesThroughRdfsDomain(this IUriNode node)
        {
            IUriNode rdfsDomain = node.Graph.CreateUriNode(RDFS.domain);
            foreach (Triple t in node.Graph.GetTriplesWithPredicateObject(rdfsDomain, node))
            {
                if (t.Subject.NodeType == NodeType.Uri)
                {
                    yield return (IUriNode)t.Subject;
                }
            }
        }

        /// <summary>
        /// Returns RDF types for this URI node.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns>All URI nodes that are asserted to be <c>rdf:type</c> for the input node.</returns>
        public static IEnumerable<IUriNode> DirectRdfTypes(this IUriNode node)
        {
            IUriNode rdfType = node.Graph.CreateUriNode(RDF.type);
            return node.Graph.GetTriplesWithSubjectPredicate(node, rdfType).Select(trip => trip.Object).UriNodes();
        }

        /// <summary>
        /// Returns RDF types, and all their transitive superclasses, for this URI node.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns>All URI nodes that are asserted to be <c>rdf:type</c> for the input node, or that are transitively asserted to be RDFS superclasses of such type nodes.</returns>
        public static IEnumerable<IUriNode> TransitiveRdfTypes(this IUriNode node)
        {
            IEnumerable<IUriNode> directRdfTypes = node.DirectRdfTypes();
            IEnumerable<IUriNode> superClassesOfDirectRdfTypes = directRdfTypes.SelectMany(rdfType => rdfType.TransitiveSuperClasses());
            return directRdfTypes.Union(superClassesOfDirectRdfTypes);
        }

        /// <summary>
        /// Returns direct RDFS superclasses for this URI node.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns>All URI nodes that the input node is asserted to be <c>rdfs:subClassOf</c>.</returns>
        public static IEnumerable<IUriNode> DirectSuperClasses(this IUriNode node)
        {
            IUriNode rdfsSubClassOf = node.Graph.CreateUriNode(RDFS.subClassOf);
            foreach (Triple t in node.Graph.GetTriplesWithSubjectPredicate(node, rdfsSubClassOf))
            {
                if (t.Object.NodeType == NodeType.Uri)
                {
                    yield return (IUriNode)t.Object;
                }
            }
        }

        /// <summary>
        /// Returns direct and transitive superclasses for this URI node.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns>All URI nodes that the input node is asserted to be <c>rdfs:subClassOf</c>,
        /// and transitively, all nodes that those parent nodes are <c>rdfs:subClassOf</c>, and so on.</returns>
        public static IEnumerable<IUriNode> TransitiveSuperClasses(this IUriNode node)
        {
            IEnumerable<IUriNode> directSuperClasses = node.DirectSuperClasses();
            HashSet<IUriNode> allSuperClasses = new HashSet<IUriNode>();
            allSuperClasses.UnionWith(directSuperClasses);
            foreach (IUriNode superClass in directSuperClasses)
            {
                allSuperClasses.UnionWith(superClass.TransitiveSuperClasses());
            }

            return allSuperClasses;
        }

        /// <summary>
        /// Returns direct RDFS subclasses for this URI node.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns>All URI nodes that are asserted to be <c>rdfs:subClassOf</c> the input node.</returns>
        public static IEnumerable<IUriNode> DirectSubClasses(this IUriNode node)
        {
            IUriNode rdfsSubClassOf = node.Graph.CreateUriNode(RDFS.subClassOf);
            foreach (Triple t in node.Graph.GetTriplesWithPredicateObject(rdfsSubClassOf, node))
            {
                if (t.Object.NodeType == NodeType.Uri)
                {
                    yield return (IUriNode)t.Subject;
                }
            }
        }

        /// <summary>
        /// Returns direct and transitive subclasses for this URI node.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns>All URI nodes that are asserted to be <c>rdfs:subClassOf</c> the input node,
        /// and transitively, all nodes that are <c>rdfs:subClassOf</c> those nodes, and so on.</returns>
        public static IEnumerable<IUriNode> TransitiveSubClasses(this IUriNode node)
        {
            IEnumerable<IUriNode> directSubClasses = node.DirectSubClasses();
            HashSet<IUriNode> allSubClasses = new HashSet<IUriNode>();
            allSubClasses.UnionWith(directSubClasses);
            foreach (IUriNode subClass in directSubClasses)
            {
                allSubClasses.UnionWith(subClass.TransitiveSubClasses());
            }

            return allSubClasses;
        }

        /// <summary>
        /// Returns instances that are RDF typed as members of this node.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns>All nodes that are asserted to have this node as their <c>rdf:type</c>.</returns>
        public static IEnumerable<INode> RdfTypedInstances(this IUriNode node)
        {
            IUriNode rdfType = node.Graph.CreateUriNode(RDF.type);
            return node.Graph.GetTriplesWithPredicateObject(rdfType, node).Subjects();
        }

        /// <summary>
        /// Checks if this URI node is an OWL Object Property.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns><c>true</c> if this node has an rdf:type, directly or transitively that is owl:ObjectProperty.</returns>
        public static bool IsObjectProperty(this IUriNode node)
        {
            return node.TransitiveRdfTypes().Any(rdfType => rdfType.Uri.AbsoluteUri.Equals(OWL.ObjectProperty.AbsoluteUri));
        }

        /// <summary>
        /// Checks if this URI node is an OWL Object Property.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns><c>true</c> if this node has an rdf:type, directly or transitively that is owl:DataProperty.</returns>
        public static bool IsDataProperty(this IUriNode node)
        {
            return node.TransitiveRdfTypes().Any(rdfType => rdfType.Uri.AbsoluteUri.Equals(OWL.DatatypeProperty.AbsoluteUri));
        }

        /// <summary>
        /// Returns any URI nodes that are asserted to be in the <c>rdfs:range</c> of this node; under RDFS semantics implying that
        /// this node is an rdf:property.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns>All URI nodes asserted to be <c>rdfs:range</c> for this node.</returns>
        public static IEnumerable<IUriNode> RdfsRanges(this IUriNode node)
        {
            IUriNode rdfsRange = node.Graph.CreateUriNode(RDFS.range);
            return node.Graph.GetTriplesWithSubjectPredicate(node, rdfsRange).Select(triple => triple.Object).UriNodes();
        }

        /// <summary>
        /// Syntactic sugar to extract subject nodes from this triple collection.
        /// </summary>
        /// <param name="triples">Input triple collection.</param>
        /// <returns>Subject nodes of the triples.</returns>
        public static IEnumerable<INode> Subjects(this IEnumerable<Triple> triples)
        {
            return triples.Select(triple => triple.Subject);
        }

        /// <summary>
        /// Syntactic sugar to extract predicate nodes from this triple collection.
        /// </summary>
        /// <param name="triples">Input triple collection.</param>
        /// <returns>Predicate nodes of the triples.</returns>
        public static IEnumerable<INode> Predicates(this IEnumerable<Triple> triples)
        {
            return triples.Select(triple => triple.Predicate);
        }

        /// <summary>
        /// Syntactic sugar to extract object nodes from this triple collection.
        /// </summary>
        /// <param name="triples">Input triple collection.</param>
        /// <returns>Object nodes of the triples.</returns>
        public static IEnumerable<INode> Objects(this IEnumerable<Triple> triples)
        {
            return triples.Select(triple => triple.Object);
        }
    }
}