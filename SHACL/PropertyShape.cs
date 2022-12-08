// <copyright file="PropertyShape.cs" company="RealEstateCore Consortium">
// Copyright (c) RealEstateCore Consortium. All rights reserved.
// </copyright>

namespace RealEstateCore.DotNetRdfExtensions.SHACL
{
    using VDS.RDF;
    using VDS.RDF.Parsing;
    using VDS.RDF.Shacl;

    public class PropertyShape : Shape
    {
        public PropertyShape(INode node, ShapesGraph graph)
            : base(node, graph)
        {
        }

        public INode Path
        {
            get
            {
                IUriNode shPath = this.Graph.CreateUriNode(SH.path);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shPath).First().Object;
            }
        }

        public IUriNode? Datatype
        {
            get
            {
                IUriNode shDatatype = this.Graph.CreateUriNode(SH.datatype);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shDatatype).Select(trip => trip.Object).UriNodes().FirstOrDefault();
            }
        }

        public IEnumerable<IUriNode> Class
        {
            get
            {
                IUriNode shClass = this.Graph.CreateUriNode(SH.cls);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shClass).Select(trip => trip.Object).UriNodes();
            }
        }

        public IEnumerable<INode> In
        {
            get
            {
                IUriNode shIn = this.Graph.CreateUriNode(SH.In);
                IEnumerable<INode> inListRoots = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shIn).Objects();
                return inListRoots.SelectMany(listItem => this.Graph.GetListItems(listItem));
            }
        }

        public IEnumerable<ILiteralNode> Names
        {
            get
            {
                IUriNode shName = this.Graph.CreateUriNode(SH.name);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shName).Objects().LiteralNodes();
            }
        }

        public IEnumerable<ILiteralNode> Descriptions
        {
            get
            {
                IUriNode shDescription = this.Graph.CreateUriNode(SH.description);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shDescription).Objects().LiteralNodes();
            }
        }

        public int? MinCount
        {
            get
            {
                IUriNode shMinCount = this.Graph.CreateUriNode(SH.minCount);
                IEnumerable<int> minCounts = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shMinCount)
                    .Objects()
                    .LiteralNodes()
                    .Select(node => int.Parse(node.Value));
                if (minCounts.Count() == 1)
                {
                    return minCounts.First();
                }
                else
                {
                    return null;
                }
            }

            set
            {
                IUriNode shMinCount = this.Graph.CreateUriNode(SH.minCount);
                var currentMinCountTriples = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shMinCount).ToList();
                this.Graph.Retract(currentMinCountTriples);
                if (value != null)
                {
                    ILiteralNode minCountNode = this.Graph.CreateLiteralNode(value.ToString(), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeInteger));
                    this.Graph.Assert(this.Node, shMinCount, minCountNode);
                }
            }
        }

        public int? MaxCount
        {
            get
            {
                IUriNode shMaxCount = this.Graph.CreateUriNode(SH.maxCount);
                IEnumerable<int> maxCounts = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shMaxCount)
                    .Objects()
                    .LiteralNodes()
                    .Select(node => int.Parse(node.Value));
                if (maxCounts.Count() == 1)
                {
                    return maxCounts.First();
                }
                else
                {
                    return null;
                }
            }

            set
            {
                IUriNode shMaxCount = this.Graph.CreateUriNode(SH.maxCount);
                var currentMaxCountTriples = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shMaxCount).ToList();
                this.Graph.Retract(currentMaxCountTriples);
                if (value != null)
                {
                    ILiteralNode maxCountNode = this.Graph.CreateLiteralNode(value.ToString(), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeInteger));
                    this.Graph.Assert(this.Node, shMaxCount, maxCountNode);
                }
            }
        }

        public void AddIn(INode inNode)
        {
            IUriNode shIn = this.Graph.CreateUriNode(SH.In);
            List<INode> newInList = new () { inNode };
            INode? existingInListRoot = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shIn).Objects().FirstOrDefault();
            if (existingInListRoot != null)
            {
                this.Graph.AddToList(existingInListRoot, newInList);
            }
            else
            {
                INode newInListRoot = this.Graph.AssertList(newInList);
                this.Graph.Assert(this.Node, shIn, newInListRoot);
            }
        }

        public void AddDatatype(Uri dt)
        {
            IUriNode shDatatype = this.Graph.CreateUriNode(SH.datatype);
            IUriNode dtNode = this.Graph.CreateUriNode(dt);
            this.Graph.Assert(this.Node, shDatatype, dtNode);
        }

        public void AddClass(Uri cls)
        {
            IUriNode shClass = this.Graph.CreateUriNode(SH.cls);
            IUriNode clsNode = this.Graph.CreateUriNode(cls);
            this.Graph.Assert(this.Node, shClass, clsNode);
        }

        public void AddName(string language, string value)
        {
            IUriNode shName = this.Graph.CreateUriNode(SH.name);
            ILiteralNode nameNode = this.Graph.CreateLiteralNode(value, language);
            this.Graph.Assert(this.Node, shName, nameNode);
        }

        public void AddNode(Uri node)
        {
            IUriNode nodeTarget = this.Graph.CreateUriNode(node);
            IUriNode shNode = this.Graph.CreateUriNode(SH.node);
            this.Graph.Assert(this.Node, shNode, nodeTarget);
        }

        public void AddDescription(string language, string value)
        {
            IUriNode shDescription = this.Graph.CreateUriNode(SH.description);
            ILiteralNode descriptionNode = this.Graph.CreateLiteralNode(value, language);
            this.Graph.Assert(this.Node, shDescription, descriptionNode);
        }
    }
}