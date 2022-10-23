using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] TMP_Text stepText;
    [SerializeField] ParticleSystem dieParticle;
    [SerializeField, Range(0.01f, 1f)] float moveDuration = 0.2f;
    [SerializeField, Range(0.01f, 1f)] float jumpHight = 0.5f;
    private float backBoundary;
    private float leftBoundary;
    private float rightBoundary;

    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private AudioSource DieSoundEffect;

    [SerializeField] private int maxTravel;
    public int MaxTravel { get => maxTravel; }
    [SerializeField] private int currentTravel;
    public int CurrentTravel { get => currentTravel; }
    public bool IsDie { get => this.enabled == false; }

    public void SetUp(int minZPos, int extent)
    {
        backBoundary = minZPos - 1;
        leftBoundary = -(extent + 1);
        rightBoundary = extent + 1;
    }

    private void Update()
    {
        var moveDir = Vector3.zero;
        // * Maju depan belakang
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDir += new Vector3(0, 0, 1);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir += new Vector3(0, 0, -1);
        }
        // * Samping kanan kiri
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDir += new Vector3(1, 0, 0);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDir += new Vector3(-1, 0, 0);
        }

        if (moveDir != Vector3.zero && IsJumping() == false)
        {
            jumpSoundEffect.Play();
            Jump(moveDir);
        }
    }

    private void Jump(Vector3 targetDirection)
    {
        // atur rotasi
        Vector3 targetPosition = transform.position + targetDirection;
        transform.LookAt(targetPosition);
        // loncat ke atas
        var moveSeq = DOTween.Sequence(transform);
        moveSeq.Append(transform.DOMoveY(jumpHight, moveDuration / 2));
        moveSeq.Append(transform.DOMoveY(0, moveDuration / 2));

        if (targetPosition.z <= backBoundary ||
        targetPosition.x <= leftBoundary ||
        targetPosition.x >= rightBoundary)
            return;

        if (Tree.AllPositions.Contains(targetPosition))
            return;


        // gerak maju/mundur
        transform.DOMoveX(targetPosition.x, moveDuration);
        transform
        .DOMoveZ(targetPosition.z, moveDuration)
        .OnComplete(UpdateTravel);

    }

    private void UpdateTravel()
    {
        currentTravel = (int)this.transform.position.z;
        if (currentTravel > maxTravel)
            maxTravel = currentTravel;

        stepText.text = "STEP : " + maxTravel.ToString();
    }

    public bool IsJumping()
    {
        return DOTween.IsTweening(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.enabled == false)
            return;

        // dijalankan sekali pada frame saat bersentuhan
        var car = other.GetComponent<Car>();
        if (car != null)
        {
            AnimateCrash(car);
        }

        // if (other.tag == "Car")
        // {
        // AnimateDie();
        // }
    }

    private void AnimateCrash(Car car)
    {
        // var isRight = car.transform.rotation.y == 90;

        // transform.DOMoveX(isRight ? 4 : -4, 2f);
        // transform
        //     .DORotate(Vector3.forward * 360, 1f)
        //     .SetLoops(10, LoopType.Restart);
        // Gepeng
        DieSoundEffect.Play();
        transform.DOScaleY(0.2f, 0.2f);
        transform.DOScaleX(3, 0.2f);
        transform.DOScaleZ(2, 0.2f);
        this.enabled = false;
        dieParticle.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        // dijalankan setiap frame selama masih menempel
    }

    private void OnTriggerExit(Collider other)
    {
        // dijalankan sekali pada frame saat tidak menempel
    }

}
