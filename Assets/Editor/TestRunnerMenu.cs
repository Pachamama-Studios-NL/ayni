using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

public class TestRunnerMenu : ICallbacks
{
    private static TestRunnerApi _api;

    [MenuItem("Tools/Tests/Run EditMode")]
    public static void RunEditMode()
    {
        EnsureApi();
        var filter = new Filter { testMode = TestMode.EditMode };
        _api.Execute(new ExecutionSettings(filter));
    }

    [MenuItem("Tools/Tests/Run PlayMode")]
    public static void RunPlayMode()
    {
        EnsureApi();
        var filter = new Filter { testMode = TestMode.PlayMode };
        _api.Execute(new ExecutionSettings(filter));
    }

    private static void EnsureApi()
    {
        if (_api == null)
        {
            _api = ScriptableObject.CreateInstance<TestRunnerApi>();
            _api.RegisterCallbacks(new TestRunnerMenu());
        }
    }

    // ICallbacks
    public void RunStarted(ITestAdaptor testsToRun)
    {
        Debug.Log($"Test run started: {testsToRun.Name}");
    }

    public void RunFinished(ITestResultAdaptor result)
    {
        Debug.Log($"Test run finished. Passed: {result.PassCount}, Failed: {result.FailCount}, Skipped: {result.SkipCount}");
    }

    public void TestStarted(ITestAdaptor test)
    {
        // optional verbose logging
    }

    public void TestFinished(ITestResultAdaptor result)
    {
        if (result.TestStatus == TestStatus.Failed)
        {
            Debug.LogError($"Test failed: {result.Name} -> {result.Message}\n{result.StackTrace}");
        }
    }
}

