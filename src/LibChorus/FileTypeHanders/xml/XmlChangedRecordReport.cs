using System.Xml;
using Chorus.merge;
using Chorus.VcsDrivers.Mercurial;

namespace Chorus.FileTypeHanders.xml
{
	/// <summary>
	/// THis may only be useful for quick, high-level identification that an entry changed,
	/// leaving *what* changed to a second pass, if needed by the user
	/// </summary>
	public class XmlChangedRecordReport : ChangeReport, IChangeReport, IXmlChangeReport
	{
		private readonly XmlNode _parent;
		private readonly XmlNode _child;
		private readonly string _url;

		public XmlChangedRecordReport(FileInRevision parentFileInRevision, FileInRevision childFileInRevision, XmlNode parent, XmlNode child)
			: this(parentFileInRevision,childFileInRevision, parent,child, string.Empty)
		{
			_parent = parent;
			_child = child;
		}
		public XmlChangedRecordReport(FileInRevision parentFileInRevision, FileInRevision childFileInRevision, XmlNode parent, XmlNode child,string url)
			: base(parentFileInRevision, childFileInRevision)
		{
			_parent = parent;
			_child = child;
			_url = url;
		}

		public override string UrlOfItem
		{
			get
			{
				return _url;
			}
		}

		public override string ActionLabel
		{
			get { return "Change"; }
		}

		public XmlNode ParentNode
		{
			get { return _parent; }
		}

		public XmlNode ChildNode
		{
			get { return _child; }
		}
	}
}