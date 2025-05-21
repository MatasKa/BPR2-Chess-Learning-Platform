using UnityEngine;
using Unity.Barracuda;
using System.Collections.Generic;

public class ChessAI : MonoBehaviour
{
    [Header("Assign your ONNX here (Barracuda will convert it)")]
    public NNModel policyOnnxAsset;

    private Model   _runtimeModel;
    private IWorker _worker;
    private Dictionary<int,string> _intToMove;

    void Awake()
    {
        // 1) Load the converted Barracuda model (not raw bytes)
        _runtimeModel = ModelLoader.Load(policyOnnxAsset);
        _worker       = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, _runtimeModel);

        // 2) Load your mapping JSON as before...
        var jsonText = System.IO.File.ReadAllText(
            System.IO.Path.Combine(Application.streamingAssetsPath, "ChessAI/move_mapping.json")
        );
        var stringDict = Newtonsoft.Json.JsonConvert
                            .DeserializeObject<Dictionary<string,string>>(jsonText);
        _intToMove = new Dictionary<int, string>();
        foreach (var kv in stringDict)
        {
            _intToMove[int.Parse(kv.Key)] = kv.Value;
        }
    }

    public string PredictMove(float[,,] boardTensor)
    {
        // Flatten the 3D array to 1D
        int d0 = boardTensor.GetLength(0);
        int d1 = boardTensor.GetLength(1);
        int d2 = boardTensor.GetLength(2);
        float[] flat = new float[d0 * d1 * d2];
        int idx = 0;
        for (int i = 0; i < d0; i++)
            for (int j = 0; j < d1; j++)
                for (int k = 0; k < d2; k++)
                    flat[idx++] = boardTensor[i, j, k];

        using var input = new Tensor(1, 8, 8, 12, flat);
        _worker.Execute(input);
        using Tensor output = _worker.PeekOutput();

        // find max index
        int best = 0; float max = float.MinValue;
        for(int i=0;i<output.length;i++)
            if(output[i]>max){ max=output[i]; best=i; }

        return _intToMove[best];
    }

    void OnDestroy() => _worker?.Dispose();
}
