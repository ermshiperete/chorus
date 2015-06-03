﻿// Copyright (c) 2015 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Chorus.FileTypeHanders.test;
using Chorus.Utilities.code;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using System.ComponentModel.Composition.Primitives;

namespace Chorus.FileTypeHanders
{
	/// <summary>
	/// Chorus file type handler collection.
	/// </summary>
	public class ChorusFileTypeHandlerCollection
	{
		/// <summary>
		/// Gets the list of handlers
		/// </summary>
		[ImportMany]
		public IEnumerable<IChorusFileTypeHandler> Handlers { get; private set; }

		private ChorusFileTypeHandlerCollection(
			Expression<Func<ComposablePartDefinition, bool>> filter = null,
			string[] additionalAssemblies = null)
		{
			var libChorusAssembly = Assembly.GetExecutingAssembly();

			//Set the codebase variable appropriately depending on the OS
			var codeBase = libChorusAssembly.CodeBase.Substring(LinuxUtils.IsUnix ? 7 : 8);

			var dirname = Path.GetDirectoryName(codeBase);
			//var baseDir = new Uri(dirname).AbsolutePath; // NB: The Uri class in Windows and Mono are not the same.
			var baseDir = dirname;

			using (var aggregateCatalog = new AggregateCatalog())
			{
				aggregateCatalog.Catalogs.Add(new AssemblyCatalog(libChorusAssembly));
				aggregateCatalog.Catalogs.Add(new DirectoryCatalog(baseDir, "*-ChorusPlugin.dll"));
				if (additionalAssemblies != null)
				{
					foreach (var assemblyPath in additionalAssemblies)
						aggregateCatalog.Catalogs.Add(new AssemblyCatalog(assemblyPath));
				}

				ComposablePartCatalog catalog;
				if (filter != null)
					catalog = new FilteredCatalog(aggregateCatalog, filter);
				else
					catalog = aggregateCatalog;

				using (var container = new CompositionContainer(catalog))
				{
					container.ComposeParts(this);
				}
			}
		}

		/// <summary/>
		public static ChorusFileTypeHandlerCollection CreateWithInstalledHandlers(
			string[] additionalAssemblies = null)
		{
			return new ChorusFileTypeHandlerCollection(additionalAssemblies: additionalAssemblies);
		}

		/// <summary/>
		public static ChorusFileTypeHandlerCollection CreateWithTestHandlerOnly()
		{
			return new ChorusFileTypeHandlerCollection(def => def.Metadata.ContainsKey("Scope") &&
				def.Metadata["Scope"].ToString() == "UnitTest");
		}

		/// <summary/>
		public IChorusFileTypeHandler GetHandlerForMerging(string path)
		{
			var handler = Handlers.FirstOrDefault(h => h.CanMergeFile(path));
			return handler ?? new DefaultFileTypeHandler();
		}
		/// <summary/>
		public IChorusFileTypeHandler GetHandlerForDiff(string path)
		{
			var handler = Handlers.FirstOrDefault(h => h.CanDiffFile(path));
			return handler ?? new DefaultFileTypeHandler();
		}
		/// <summary/>
		public IChorusFileTypeHandler GetHandlerForPresentation(string path)
		{
			var handler = Handlers.FirstOrDefault(h => h.CanPresentFile(path));
			return handler ?? new DefaultFileTypeHandler();
		}
	}
}

