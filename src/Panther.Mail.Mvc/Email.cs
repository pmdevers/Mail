using System;
using System.Dynamic;

using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Panther.Mail.Mvc
{
    public class Email : DynamicObject
    { 
        //internal ImageEmbedder ImageEmbedder { get; private set; }
        public ViewDataDictionary ViewData { get; set; }

        [FromServices]
        public ITempDataDictionary TempData { get; set; }
        public string ViewName { get; set; }

        //public List<Attachment> Attachments { get; set; }

        public Email()
        {
            //Attachments = new List<Attachment>();
            ViewName = DeriveViewNameFromClassName();
            ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = this };
            //ImageEmbedder = new ImageEmbedder();
        }

        public Email(string viewName)
        {
            if(string.IsNullOrEmpty(viewName))
                throw new ArgumentNullException(nameof(viewName));

            //Attachments = new List<Attachment>();
            ViewName = viewName;
            ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = this };
            //ImageEmbedder = new ImageEmbedder();
        }

        //public void Attach(Attachment attachment)
        //{
        //    Attachments.Add(attachment);
        //}

        private string DeriveViewNameFromClassName()
        {
            string viewName = GetType().Name;
            if (viewName.EndsWith("Email"))
            {
                viewName = viewName.Substring(0, viewName.Length - "Email".Length);
            }
            return viewName;
        }
    }
}