using System;
using System.Collections.Generic;
using System.Text;

namespace Jobb.Generator
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomToolAttribute : Attribute
    {
        /// <summary></summary>
        protected string _description;

        /// <summary></summary>
        protected string _name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public CustomToolAttribute(string name) :
          this(name, "")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public CustomToolAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return _description; }
        }
    }
}
