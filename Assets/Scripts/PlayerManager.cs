using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerManager : MonoBehaviour
{
    public Transform player;
    private int nunberOfStickmans;
    [SerializeField] private TextMeshPro CounterTxt;
    [SerializeField] private GameObject stickMan;



    Quaternion rotation = new Quaternion(0.0f, -1.0f, 0.0f, 0.00000314414456f);
    Vector3 scale = new Vector3(0.0204742271f, 0.0204742271f, 0.0204742271f);

    //--*-*-*-**-*-*-**-*-*-*--*-*-**-*-**-*--**-**-*-*-*
    [Range(0f, 1f)][SerializeField] private float DistanceFactor, Radius;


    //--*-*-*-**-*-*-**-*-*-*--*-*-**-*-**-*--**-**-*-*-*

    public bool moveByTouch, gameState;
    private Vector3 mouseStartPos, playerStartPos;
    public float playerSpeed, roadSpeed;
    private Camera camera;

    [SerializeField] private Transform road;
    [SerializeField] private Transform _enemy;
    private bool attack;


    void Start()
    {
        player = transform;
        nunberOfStickmans = transform.childCount - 1;

        CounterTxt.text = nunberOfStickmans.ToString();
        camera = Camera.main;
    }

    void Update()
    {
        if (attack)
        {
            var enemyDirection = _enemy.position - player.position; // Düşmana doğru olan yön

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                // Rotation işlemi için engel
                if (child.name == "Square" || child.name == "runnerTxt") 
                    continue;

                // Her bir karakteri düşmana doğru döndür
                Vector3 directionToEnemy = _enemy.position - child.position;
                child.rotation = Quaternion.Slerp(
                    child.rotation,
                    Quaternion.LookRotation(directionToEnemy, Vector3.up),
                    Time.deltaTime * 5); // Rotasyon hızını artırın
            }

            if (_enemy.GetChild(1).childCount > 1)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);

                    // Rotation işlemi için engel
                    if (child.name == "Square" || child.name == "runnerTxt") 
                        continue;

                    var distance = _enemy.GetChild(1).GetChild(0).position - child.position;
                    if (distance.magnitude < 1.5f)
                    {
                        float attackSpeed = 8f; // Saldırı hızı faktörü
                        child.position = Vector3.Lerp(
                            child.position,
                            new Vector3(
                                _enemy.GetChild(1).GetChild(0).position.x,
                                child.position.y,
                                _enemy.GetChild(1).GetChild(0).position.z),
                            Time.deltaTime * attackSpeed); // Hız faktörünü artırın
                    }
                }
            }
            else
            {
                attack = false;
                roadSpeed = 7f;
                for (int i = 1; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);

                    // Rotation işlemi için engel
                    if (child.name == "Square" || child.name == "runnerTxt") 
                        continue;

                    child.rotation = Quaternion.Slerp(child.rotation, Quaternion.identity, Time.deltaTime * 7f);
                }
                FormatStickMan();
                _enemy.gameObject.SetActive(false);
            }
        }
        else
        {
            MoveThePlayer();
        }
        if (gameState)
        {
            road.Translate(road.forward * Time.deltaTime * roadSpeed);
            for (int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Animator>().SetBool("run", true);
            }
        }
    }


    void MoveThePlayer()
    {
        if (Input.GetMouseButtonDown(0) && gameState)
        {
            moveByTouch = true;
            var plane = new Plane(Vector3.up, 0f);
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out var distance))
            {
                mouseStartPos = ray.GetPoint(distance * 1f);
                playerStartPos = transform.position;
            }
        }
        if (Input.GetMouseButton(0))
        {
            moveByTouch = false;
        }
        if (moveByTouch)
        {
            var plane = new Plane(Vector3.up, 0f);
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var distance))
            {
                var mousePos = ray.GetPoint(distance * 1f);
                var move = mousePos - playerStartPos;
                var control = playerStartPos + move;
                //limit eğer 50 den fazla oyuncu varsa daha da az hareket etsinler
                if (nunberOfStickmans > 50)
                    control.x = Mathf.Clamp(control.x, -2f, 2f);
                else
                    control.x = Mathf.Clamp(control.x, -3f, 3f);

                transform.position = new Vector3(Mathf.Lerp(transform.position.x, control.x, Time.deltaTime * playerSpeed), transform.position.y, transform.position.z);
            }
        }
     
    }

    private void FormatStickMan()
    {
        for (int i = 1; i < player.childCount; i++)
        {
            var x = DistanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = DistanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);

            var NewPos = new Vector3(x, -0.08801528f, z);

            player.transform.GetChild(i).DOLocalMove(NewPos, 1f).SetEase(Ease.OutBack);

            Vector3 directionToLook = Vector3.zero - player.transform.GetChild(i).position; // Vector3.zero yönüne olan vektör
            player.transform.GetChild(i).DOLocalRotateQuaternion(Quaternion.LookRotation(directionToLook), 1f).SetEase(Ease.OutBack);
        }
    }

    private void MakeStickMan(int number)
    {
        if (number > 0)
        {
            // Karakter ekleme
            for (int i = 0; i < number; i++)
            {
                GameObject newStickMan = Instantiate(stickMan, transform.position, Quaternion.identity, transform);
                newStickMan.transform.localScale = scale;
            }
        }
        else if (number < 0)
        {
            //silme işlemine tekrardan bakılması lazım !!!!!!!!!!!!!!!!
            int charactersToRemove = Mathf.Abs(number);

            if (nunberOfStickmans - charactersToRemove < 1)
            {
                charactersToRemove = nunberOfStickmans - 1; // En az 1 karakter bırak
            }

            for (int i = 0; i < charactersToRemove; i++)
            {
                if (transform.childCount > 1)
                {
                    Transform stickmanToRemove = transform.GetChild(transform.childCount - 1);
                    stickmanToRemove.DOKill(); 
                    Destroy(stickmanToRemove.gameObject);
                }

            }
        }

        nunberOfStickmans = transform.childCount - 1;
        CounterTxt.text = nunberOfStickmans.ToString();

        FormatStickMan();
    }





    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("gate"))
        {
            // Çarpılan nesnenin bir üst parent'ını buluyoruz
            Transform gateParent = other.transform.parent;

            // Parent içindeki Gate1 ve Gate2'yi buluyoruz
            Transform firstGate = gateParent.Find("Gate1");
            Transform secondGate = gateParent.Find("Gate2");

            // İlk kapıyı kapatıyoruz
            if (firstGate != null)
            {
                firstGate.GetComponent<BoxCollider>().enabled = false;
            }

            // İkinci kapıyı kapatıyoruz
            if (secondGate != null)
            {
                secondGate.GetComponent<BoxCollider>().enabled = false;
            }

            var gateManager = other.GetComponent<GateManager>();

            int newStickmanCount;

            if (gateManager.multiply)
            {
                newStickmanCount = nunberOfStickmans * Mathf.Abs(gateManager.randomNumber);
            }
            else
            {
                newStickmanCount = nunberOfStickmans + gateManager.randomNumber;
            }

            // Yeni karakter sayısını oluşturuyoruz
            MakeStickMan(newStickmanCount - nunberOfStickmans);
        }

        if (other.CompareTag("enemy"))
        {
            _enemy = other.transform;
            attack = true;

            roadSpeed = 4f;
            other.transform.GetChild(1).GetComponent<enemyManager>().AttackThem(transform);
        }
    }


}
