using UnityEngine;

public class CarroControle : MonoBehaviour
{
    public float aceleracao = 6f;
    public float desaceleracao = 6f;
    public float velocidadeMaxima = 12f;
    public float rotacaoSuavizada = 200f;

    private Rigidbody2D rb;
    private float velocidadeAtual = 0f;
    private float inputMovimento = 0f;
    private float inputRotacao = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.up * velocidadeAtual;

        if (Mathf.Abs(velocidadeAtual) > 0.1f)
        {
            float rotacaoAjustada = -inputRotacao * rotacaoSuavizada * Time.fixedDeltaTime;
            transform.Rotate(0, 0, rotacaoAjustada);
        }

        if (inputMovimento == 0)
        {
            if (velocidadeAtual > 0)
                velocidadeAtual = Mathf.Max(0, velocidadeAtual - desaceleracao * Time.fixedDeltaTime);
        }

        velocidadeAtual = Mathf.Clamp(velocidadeAtual, 0, velocidadeMaxima);
    }

    public void SetInputs(float movimento, float rotacao)
    {
        inputMovimento = Mathf.Clamp01(movimento);
        inputRotacao = Mathf.Clamp(rotacao, -1f, 1f);

        if (inputMovimento > 0)
            velocidadeAtual += aceleracao * Time.fixedDeltaTime;
    }
}