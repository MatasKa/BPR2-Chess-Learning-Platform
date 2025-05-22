using UnityEngine;
using Unity.Barracuda;
using System.Collections.Generic;

public class ChessAI : MonoBehaviour
{
    //[Header("Assign your ONNX here (Barracuda will convert it)")]
    public NNModel policyOnnxAsset;

    private Model _runtimeModel;
    private IWorker _worker;
    private Dictionary<int, string> _intToMove;
    private List<string> moveList;

    void Awake()
    {
        // 1) Load the converted Barracuda model
        _runtimeModel = ModelLoader.Load(policyOnnxAsset);
        _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, _runtimeModel);

        // 2) Load the move mapping JSON and populate both dictionary and list
        var jsonText = System.IO.File.ReadAllText(
            System.IO.Path.Combine(Application.streamingAssetsPath, "ChessAI/move_mapping.json")
        );
        var stringDict = Newtonsoft.Json.JsonConvert
                            .DeserializeObject<Dictionary<string, string>>(jsonText);

        _intToMove = new Dictionary<int, string>();
        moveList = new List<string>();

        // Sort keys numerically and build moveList accordingly
        var sortedKeys = new List<int>();
        foreach (var kv in stringDict)
        {
            int index = int.Parse(kv.Key);
            sortedKeys.Add(index);
            _intToMove[index] = kv.Value;
        }

        sortedKeys.Sort();
        foreach (int i in sortedKeys)
        {
            moveList.Add(_intToMove[i]);
        }
    }

    public string PredictMove(float[,,] boardTensor)
    {
        float[] flat = FlattenTensor(boardTensor);
        using var input = new Tensor(1, 8, 8, 12, flat);
        _worker.Execute(input);
        using Tensor output = _worker.PeekOutput();

        int best = 0;
        float max = float.MinValue;
        for (int i = 0; i < output.length; i++)
        {
            if (output[i] > max) { max = output[i]; best = i; }
        }

        return _intToMove[best];
    }

    public string[] PredictTopMoves(float[,,] inputTensor, int topN = 10)
    {
        float[] flat = FlattenTensor(inputTensor);
        using var tensor = new Tensor(1, 8, 8, 12, flat);
        _worker.Execute(tensor);
        using Tensor output = _worker.PeekOutput();

        var scored = new List<(int index, float value)>();
        for (int i = 0; i < output.length; i++)
        {
            scored.Add((i, output[i]));
        }

        scored.Sort((a, b) => b.value.CompareTo(a.value));

        var topMoves = new List<string>();
        for (int i = 0; i < topN && i < scored.Count; i++)
        {
            int index = scored[i].index;
            if (index >= 0 && index < moveList.Count)
                topMoves.Add(moveList[index]);
        }

        return topMoves.ToArray();
    }

    private float[] FlattenTensor(float[,,] tensor)
    {
        int d0 = tensor.GetLength(0);
        int d1 = tensor.GetLength(1);
        int d2 = tensor.GetLength(2);
        float[] flat = new float[d0 * d1 * d2];
        int idx = 0;

        for (int i = 0; i < d0; i++)
            for (int j = 0; j < d1; j++)
                for (int k = 0; k < d2; k++)
                    flat[idx++] = tensor[i, j, k];

        return flat;
    }

    void OnDestroy()
    {
        _worker?.Dispose();
    }
}
