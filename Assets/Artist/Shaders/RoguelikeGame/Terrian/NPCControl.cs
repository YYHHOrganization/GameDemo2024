using UnityEngine;

public class NPCControl : MonoBehaviour
{
    private CharacterController controller;
    private HRGPlayerRutBrush brush;

    private void Awake()
    {
        controller = this.GetComponent<CharacterController>();
        brush = this.GetComponent<HRGPlayerRutBrush>();

        this.transform.position = new Vector3(Random.Range(-30, 30), 5, Random.Range(-10, 10));
        this.transform.localScale = Random.Range(0.2f, 2) * Vector3.one;
        brush.brushRadius = this.transform.localScale.x;

        waitTime = Random.Range(0, 1);
        isWait = true;
    }

    private const float g = 9.8f;

    public Vector3 currentMoveDir;
    public float currentMaxLength;
    public Vector3 currtntOldPos;
    public float waitTime;
    public bool isWait;
    void Update()
    {
        if (!controller.isGrounded)
        {
            controller.Move((controller.velocity + g * Time.deltaTime * Vector3.down) * Time.deltaTime);
        }

        if (isWait)
        {
            waitTime += Time.deltaTime;
            if (waitTime >= 1)
            {
                currentMoveDir = new Vector3(Random.Range(0, 1.0f), 0, Random.Range(0, 1.0f)).normalized;
                currentMaxLength = Random.Range(1, 10);
                currtntOldPos = this.transform.position;
                isWait = false;
                waitTime = 0;
            }
        }
        else
        {
            if ((this.transform.position - currtntOldPos).sqrMagnitude < currentMaxLength * currentMaxLength)
            {
                controller.SimpleMove(currentMoveDir);
                if (brush)
                {
                    brush.Paint();
                }
            }
            else
            {
                isWait = true;
            }
        }
    }
}
