using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using AppContext = Models.AppContext;

namespace BL.Abstract
{
    abstract class DataHandler
    {
        protected readonly AppContext context;

        public string Message { get; protected set; }

        public DataHandler(AppContext appContext)
        {
            context = appContext;
        }

        public bool Update<T>(T item)
            where T:class
        {
            try
            {

                //await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return false;
            }

        }
    }
}
