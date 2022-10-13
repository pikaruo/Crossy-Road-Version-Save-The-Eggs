using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)] float moveDuration = 0.2f;
    [SerializeField, Range(0.01f, 1f)] float jumpHight = 0.5f;
    private void Update()
    {
        var moveDir = Vector3.zero;
        // * Maju depan belakang
        if (Input.GetKeyDown(KeyCode.RightArrow))
            moveDir += new Vector3(0, 0, 1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            moveDir += new Vector3(0, 0, -1);
        // * Samping kanan kiri
        if (Input.GetKeyDown(KeyCode.DownArrow))
            moveDir += new Vector3(1, 0, 0);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            moveDir += new Vector3(-1, 0, 0);

        if (moveDir != Vector3.zero && IsJumping() == false)
            Jump(moveDir);
    }

    private void Jump(Vector3 targetDirection)
    {
        // atur rotasi
        var TargetPosition = transform.position + targetDirection;
        transform.LookAt(TargetPosition);
        // loncat ke atas
        var moveSeq = DOTween.Sequence(transform);
        moveSeq.Append(transform.DOMoveY(jumpHight, moveDuration / 2));
        moveSeq.Append(transform.DOMoveY(0, moveDuration / 2));

        if (Tree.AllPositions.Contains(TargetPosition))
            return;

        // gerak maju/mundur
        transform.DOMoveX(TargetPosition.x, moveDuration);
        transform.DOMoveZ(TargetPosition.z, moveDuration);

    }

    private bool IsJumping()
    {
        return DOTween.IsTweening(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        // dijalankan sekali pada frame saat bersentuhan
        var car = other.GetComponent<Car>();
        if (car != null)
        {
            AnimateDie(car);
        }
        if (other.tag == "Car")
        {
            // AnimateDie();
        }
    }

    private void AnimateDie(Car car)
    {
        // var isRight = car.transform.rotation.y == 90;

        // transform.DOMoveX(isRight ? 4 : -4, 2f);
        // transform
        //     .DORotate(Vector3.forward * 360, 1f)
        //     .SetLoops(10, LoopType.Restart);
        // Gepeng
        transform.DOScaleY(0.2f, 0.2f);
        transform.DOScaleX(3, 0.2f);
        transform.DOScaleZ(2, 0.2f);
        this.enabled = false;
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
