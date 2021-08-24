using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Models;
using AppContext = Models.AppContext;

namespace BL.DbHandling
{
    public class UpdateHandler
    {
        private Thread t;
        private bool cont;
        public UpdateHandler(AppContext db)
        {
            this.db = db;
        }

        long _lastUpdate;
        private readonly AppContext db;

        bool isUpdateLocal;

        public void MakeUpdate()
        {
            _lastUpdate++;
            isUpdateLocal = true;
        }

        public void Start()
        {
            _lastUpdate = db.Updates.First().LastUpdate;
            cont = true;
            t = new Thread(Updater);
            t.Start();
        }

        public void Stop()
        {
            cont = false;
            t?.Abort();
        }

        public void OnUpdate()
        {

        }

        private void Updater()
        {
            while (cont)
            {
                var update = db.Updates.First();

                if (isUpdateLocal)
                {
                    update.LastUpdate = _lastUpdate;
                    db.SaveChanges();
                    isUpdateLocal = false;                    
                }
                else if(update.LastUpdate > _lastUpdate)
                {

                }

                Thread.Sleep(500);
            }
        }
    }
}
