using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using QiskitPlugin.Utility;
using QiskitPlugin;

public class QiskitTestor : MonoBehaviour
{
    [SerializeField] TMP_Text resultField;
    [SerializeField] TMP_Text stateVector;
    [SerializeField] TMP_Text probTables;
    int circuitIndex;

    void Start()
    {
        StartCoroutine(ScreenUpdateRoutine());
    }


    IEnumerator ScreenUpdateRoutine()
    {
        CircuitBuilder circuit;
        circuitIndex = CircuitProvider.instance.CreateCircuit(3,out circuit);

        List<ITask> tasks = new List<ITask>();
        while (true)
        {
            yield return new WaitUntil(()=>!circuit.updatedToHead);
            yield return new WaitUntil(()=>circuit.updatedToHead);

            var qc = circuit.BuildCircuit();
            var probTask = qc.GetStateProbAsync();
            var vecTask = qc.GetStateVectorAsync();

            yield return new WaitUntil(()=>probTask.ready);
            yield return new WaitUntil(()=>vecTask.ready);

            stateVector.text = vecTask.result;
            probTables.text = string.Join(",",probTask.result);
            Debug.Log("updated");
        }

    }
    public void RunCircuit()
    {
        var task = CircuitProvider.instance.circuits[circuitIndex].BuildCircuit().RunAsync();

        StartCoroutine(CircuitTask(task));
    }

    IEnumerator CircuitTask(ITask<CircuitMeasurementResult> task)
    {
        yield return new WaitUntil(() => task.ready);
        resultField.text = task.result.GetOverview();
    }

}
