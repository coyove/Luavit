using Autodesk.Revit.DB;
using System;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ParameterInfo
	{
		private bool m_parameterIsProject;

		private bool m_parameterForType;

		private string m_parameterName;

		private BuiltInParameterGroup m_parameterGroup;

		private ParameterType m_parameterType;

		private CategorySet m_categories;

		public bool ParameterIsProject
		{
			get
			{
				return this.m_parameterIsProject;
			}
			set
			{
				this.m_parameterIsProject = value;
			}
		}

		public bool ParameterForType
		{
			get
			{
				return this.m_parameterForType;
			}
			set
			{
				this.m_parameterForType = value;
			}
		}

		public string ParameterName
		{
			get
			{
				return this.m_parameterName;
			}
			set
			{
				this.m_parameterName = value;
			}
		}

		public BuiltInParameterGroup ParameterGroup
		{
			get
			{
				return this.m_parameterGroup;
			}
			set
			{
				this.m_parameterGroup = value;
			}
		}

		public ParameterType ParameterType
		{
			get
			{
				return this.m_parameterType;
			}
			set
			{
				this.m_parameterType = value;
			}
		}

		public CategorySet Categories
		{
			get
			{
				return this.m_categories;
			}
			set
			{
				this.m_categories = value;
			}
		}

		public ParameterInfo()
		{
			this.m_parameterGroup = (BuiltInParameterGroup)( -1);
			this.m_parameterType = (ParameterType)1;
		}

		public ParameterInfo(Definition definition, ElementBinding binding)
		{
			this.m_parameterIsProject = (definition is ExternalDefinition);
			this.m_parameterForType = (binding is TypeBinding);
			this.m_parameterName = definition.Name;
			this.m_parameterGroup = definition.ParameterGroup;
			this.m_parameterType = definition.ParameterType;
			this.m_categories = binding.Categories;
		}
	}
}
