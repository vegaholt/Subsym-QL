using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using QL.Managers;
using QL.Models;

namespace QL
{
    public class QLHub : Hub
    {
        public Task Initialize()
        {
            InitializerManager.GetInstance().SendSettingsToClient();
            return null;
        }

        public Task ProgramRun()
        {
            InitializerManager.GetInstance().Run();
            return null;
        }
        public Task ProgramStop()
        {
            InitializerManager.GetInstance().Stop();
            return null;
        }

        public Task ProgramRestart()
        {
            InitializerManager.GetInstance().Restart();
            InitializerManager.GetInstance().SendSettingsToClient();
            return null;
        }

        public Task StartSimulation()
        {
            InitializerManager.GetInstance().StartSimulation();
            return null;
        }

        public Task UpdateSettings(Settings settings)
        {
            InitializerManager.GetInstance().UpdateSettings(settings);
            return null;
        }
    }
}