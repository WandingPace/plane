using UnityEngine;
using System.Collections;

public class ClickObstacle : MonoBehaviour
{
    private RaycastHit hit;
    private bool hasClicked;
    private BoxCollider _collider;
    // Use this for initialization
    private void Start()
    {
        hasClicked = false;
        if (_collider ==null)
            _collider = gameObject.GetComponent<BoxCollider>();

        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider>();
            _collider.size = Vector3.one;
            _collider.center = new Vector3(0f, 0.5f, 0f);
        }
        //UIEventListener.Get(gameObject).onClick = OnMouseDown;
    }

    // Update is called once per frame
    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0) && !hasClicked)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            Debug.Log(Time.time + " Click the obstacle..");
    //            hasClicked = true;
    //            GameController.SharedInstance.BossEnemy.Hurt(gameObject);
    //            //gameObject.SetActive(false);

    //            if (_collider)
    //                _collider.enabled = false;
    //        }
    //    }
    //}

    private void BombHit()
    {
        hasClicked = false;
        if (_collider)
            _collider.enabled = true;
        gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        //Debug.Log("OnMouseDown");

        if (!hasClicked)
        {
            hasClicked = true;
            _collider.enabled = false;
            GameController.SharedInstance.BossEnemy.Hurt(gameObject);
        }
    }
}
