using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LapManager : MonoBehaviour
{
    public static LapManager instance;
    public List<Transform> checkpoints;
    public TMP_Text voltasText;
    public TMP_Text tempoText;
    // public TMP_Text placarText; // Comentado para não aparecer por agora

    private Dictionary<GameObject, int> progressoCarro = new Dictionary<GameObject, int>();
    private Dictionary<GameObject, int> voltasCarro = new Dictionary<GameObject, int>();
    private Dictionary<GameObject, float> tempoVolta = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, string> statusCarro = new Dictionary<GameObject, string>();

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Update()
    {
        List<GameObject> carrosParaRemover = new List<GameObject>();
        foreach (var carro in progressoCarro.Keys)
        {
            if (carro == null)
            {
                carrosParaRemover.Add(carro);
                continue;
            }

            if (voltasCarro.ContainsKey(carro))
            {
                tempoVolta[carro] += Time.deltaTime;
            }
        }

        foreach (var carro in carrosParaRemover)
        {
            progressoCarro.Remove(carro);
            voltasCarro.Remove(carro);
            tempoVolta.Remove(carro);
            statusCarro.Remove(carro);
        }
    }

    public void CarroEntrouNaCorrida(GameObject carro)
    {
        if (!statusCarro.ContainsKey(carro))
        {
            statusCarro[carro] = "Em competição";
        }
    }

    public void CarroCompletouVolta(GameObject carro, float fitness)
    {
        if (statusCarro.ContainsKey(carro))
        {
            statusCarro[carro] = $"Completou volta (Fitness: {fitness:F2})";
        }
    }

    public void CarroMorreu(GameObject carro, string motivo)
    {
        if (statusCarro.ContainsKey(carro))
        {
            statusCarro[carro] = $"Morreu ({motivo})";
        }
    }

    /*
    private void AtualizarPlacar()
    {
        placarText.text = "Placar:\n";
        foreach (var carro in statusCarro.Keys)
        {
            if (carro != null)
            {
                placarText.text += $"Carro {carro.name}: {statusCarro[carro]}\n";
            }
        }
    }
    */

    public void CarroPassouCheckpoint(GameObject carro, Transform checkpoint)
    {
        if (!progressoCarro.ContainsKey(carro))
        {
            progressoCarro[carro] = 0;
            voltasCarro[carro] = 0;
            tempoVolta[carro] = 0f;
            statusCarro[carro] = "Em competição";
        }

        int indexCheckpointAtual = progressoCarro[carro];

        if (checkpoint == checkpoints[indexCheckpointAtual])
        {
            progressoCarro[carro]++;

            if (progressoCarro[carro] >= checkpoints.Count)
            {
                progressoCarro[carro] = 0;
                voltasCarro[carro]++;
                Debug.Log($"Carro {carro.name} completou {voltasCarro[carro]} voltas!");
                Debug.Log($"Tempo da última volta: {tempoVolta[carro]:F2} segundos!");

                voltasText.text = $"Voltas: {voltasCarro[carro]}";
                tempoText.text = $"Tempo: {tempoVolta[carro]:F2}s";

                statusCarro[carro] = $"Completou {voltasCarro[carro]} voltas em {tempoVolta[carro]:F2}s";

                tempoVolta[carro] = 0f;
            }
        }
    }

    public void ReiniciarCorrida()
    {
        progressoCarro.Clear();
        voltasCarro.Clear();
        tempoVolta.Clear();
        statusCarro.Clear();

        voltasText.text = "Voltas: 0";
        tempoText.text = "Tempo: 0.00s";
        // placarText.text = "Placar:\n"; // Comentado para não reiniciar o placar
    }
}