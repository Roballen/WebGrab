using Db4objects.Db4o;

namespace ConsoleGrabit.Repository
{
    public sealed class Db4oServer
    {
        private readonly string _pathToDbFile;
        private readonly int _port;

        public Db4oServer(string pathToDbFile, int port, bool autoStrart)
        {
            _pathToDbFile = pathToDbFile;
            _port = port;
            if (autoStrart)
                Start();
        }

        /// <summary>
        /// The server
        /// </summary>
        public IObjectServer Server { get; private set; }
        /// <summary>
        /// Starts the Server
        /// </summary>
        public void Start()
        {
            Server = Db4oFactory.OpenServer(_pathToDbFile, _port);
        }
        /// <summary>
        /// Stops the Server
        /// </summary>
        public void Stop()
        {
            Server.Close();
        }
        public IObjectContainer GetClient()
        {
            return Server.OpenClient();
        }
    }

}


