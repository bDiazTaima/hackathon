using UnityEngine;
using System.Collections.Generic;

public class AssistantExpressions : MonoBehaviour
{
    [SerializeField] private Material mat;
    Dictionary<Expression, Vector2> expression_coords = new Dictionary<Expression, Vector2>();

    private void Start()
    {
        mat = GameObject.FindGameObjectWithTag("expression").GetComponent<MeshRenderer>().material;
        expression_coords.Add(Expression.HAPPY, new Vector2(0, 0));
        expression_coords.Add(Expression.HAPPY_CLOSED_EYES, new Vector2(0.333f, 0));
        expression_coords.Add(Expression.WINK, new Vector2(0.666f, 0));
        expression_coords.Add(Expression.SAD, new Vector2(0, 0.333f));
        expression_coords.Add(Expression.ERROR, new Vector2(0.333f, 0.333f));
        expression_coords.Add(Expression.SERIOUS, new Vector2(0.666f, 0.333f));
        expression_coords.Add(Expression.SURPRISED, new Vector2(0, 0.666f));
        expression_coords.Add(Expression.SHOCKED, new Vector2(0.333f, 0.666f));
        expression_coords.Add(Expression.IDLE, new Vector2(0.666f, 0.666f));

        SetExpression(Expression.HAPPY);
        Invoke("SetRandomExpression", 5.0f);
    }

    private void SetExpression(Expression expression)
    {
        Vector2 coord;
        expression_coords.TryGetValue(expression, out coord);
        mat.SetVector("Offset", coord);
    }

    public void SetRandomExpression()
    {
        int rand_exp = Random.Range(0, 10);
        SetExpression((Expression)rand_exp);
        float t = Random.Range(3, 7);
        Invoke("SetRandomExpression", t);
    }
}

public enum Expression
{
    HAPPY,
    HAPPY_CLOSED_EYES,
    WINK,
    SAD,
    ERROR,
    SERIOUS,
    SURPRISED,
    SHOCKED,
    IDLE,
}
