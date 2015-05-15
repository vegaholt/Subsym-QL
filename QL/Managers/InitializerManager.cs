using Microsoft.AspNet.SignalR;
using QL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL.Managers
{

    public class InitializerManager
    {
        #region Initialize Methods
        //Hub
        private static IHubContext _hub;
        private static InitializerManager _main;
        
        //Managers
        private static SettingsManager _settingsManager;
        private static ScenarioManager _scenarioManager;
        private static TrainingManager _trainingManager;
        private static SimulationManager _simulationManager;

        //Objects
        private static List<QMap> _policy;
        private static Settings _settings;
        private static List<Scenario> _scenarioList;
        private static Scenario _scenario;
        private static Random _random;

        public static InitializerManager GetInstance()
        {
            return _main ?? (
                _main = new InitializerManager()
            );
        }

        private InitializerManager()
        {
            //Hub
            _hub = GlobalHost.ConnectionManager.GetHubContext<QLHub>();

            //Managers
            _settingsManager = new SettingsManager();
            _scenarioManager = new ScenarioManager();
            _trainingManager = new TrainingManager();
            _simulationManager = new SimulationManager();

            //Objects
            _policy = new List<QMap>();
            _settings = new Settings();
            _scenarioList = new List<Scenario>();
            _random = new Random();

            //Set initial values
            SetInitialSettings();

            //Build scenarios
            InitializeScenarios();
        }

        private void SetInitialSettings()
        {
            _settings = _settingsManager.GetInitialSettings();
        }

        private void InitializeScenarios()
        {
            _scenarioList = _scenarioManager.GetInitialScenarios();
            UpdateScenario();
        }
        #endregion

        #region Runtime Methods

        public void Run()
        {
            //Reset values
            Stop();
            _policy = _trainingManager.Train(_scenario, _settings, _random, _hub);
        }

        public void Stop()
        {
            _policy.Clear();

            //Garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void Restart()
        {
            _main = null;
        }

        public void StartSimulation()
        {
            _simulationManager.Run(_scenario, _policy, _random, _settings, _hub);
        }
        #endregion
        
        #region Core logic

        #endregion d

        #region Server Methods
        public void UpdateSettings(Settings settings)
        {
            _settings = settings;
            UpdateScenario();
        }

        private void UpdateScenario()
        {
            var index = _settings.ScenarioIndex - 1;
            if (_settings.ScenarioIndex >= _scenarioList.Count)
            {
                index = _scenarioList.Count - 1;
            }
            else if (_settings.ScenarioIndex - 1 < 0)
            {
                index = 0;
            }

            _scenario = _scenarioList.ElementAt(index);
            VisualizeScenario(_scenario);
        }
        #endregion

        #region Client methods
        public void SendSettingsToClient()
        {
            _hub.Clients.All.hubSetSettings(_settings);
        }

        private void VisualizeScenario(Scenario scenario)
        {
            var viewModel = new ViewModel
            {
                Scenario = scenario.Values,
                Agent = new Agent { State = new State { Position = new Position { Row = scenario.StartPosition.Row, Col = scenario.StartPosition.Col }, EatenFoods = new List<int>() } },
                StartPos = new Position { Row = scenario.StartPosition.Row, Col = scenario.StartPosition.Col },
                Height = scenario.Height,
                Width = scenario.Width
            };

            _hub.Clients.All.hubVisualizeScenario(viewModel);
        }

        #endregion
    }
}