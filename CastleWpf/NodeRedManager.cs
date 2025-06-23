using System.Diagnostics;

namespace CastleWpf;

internal class NodeRedManager
{
    private Process? _nodeRedProcess;
    private readonly ManualResetEventSlim _nodeRedStarted = new(false);

    public void StartNodeRed()
    {
        _nodeRedProcess = new Process();
        var startInfo = new ProcessStartInfo()
        {
            //WindowStyle = ProcessWindowStyle.Hidden,
            FileName = @"C:\Users\fabio.castello\source\CastleApp\Runtime\nodejs\node.exe",
            Arguments = @"C:\Users\fabio.castello\source\CastleApp\Runtime\node-red\node_modules\node-red\red.js --userDir C:\Users\fabio.castello\source\CastleApp\Runtime\userdata",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true
        };

        _nodeRedProcess.StartInfo = startInfo;
        _nodeRedProcess.OutputDataReceived += OnOutputDataReceived;

        _nodeRedProcess.Start();
        _nodeRedProcess.BeginOutputReadLine();
    }

    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (_nodeRedProcess == null || string.IsNullOrEmpty(e.Data))
        {
            return;
        }

        if (e.Data.Contains("Server now running at"))
        {
            _nodeRedStarted.Set();

            _nodeRedProcess.OutputDataReceived -= OnOutputDataReceived;
            _nodeRedProcess.CancelOutputRead();
        }
    }

    public Task WaitForNodeReadReadyAsync()
    {
        return Task.Run(_nodeRedStarted.Wait);
    }

    public void StopNodeRed()
    {
        if (_nodeRedProcess != null && !_nodeRedProcess.HasExited)
        {
            if (!_nodeRedStarted.IsSet)
            {
                _nodeRedProcess.OutputDataReceived -= OnOutputDataReceived;
                _nodeRedProcess.CancelOutputRead();
            }

            _nodeRedProcess.Kill();
        }
    }
}