using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForceClick;

namespace ForceClick{


public class ExampleScript : MonoBehaviour
{

        private void Start()
        {
            ClickEventsController.Instance.OnInitialPositionSet += ShowInitialClickPosition;
            ClickEventsController.Instance.OnClickHold += ShowHoldingPosition;
            ClickEventsController.Instance.OnClickUp += ShowEndPosition;
            ClickEventsController.Instance.OnClickUp += ShowInitialToEndWorldPosition;
            ClickEventsController.Instance.OnClickHold += ShowInitialToHoldWorldDirection;

            ClickEventsController.Instance.OnSwipeDown += SwipedDown;
            ClickEventsController.Instance.OnSwipeUp += SwipedUp;
            ClickEventsController.Instance.OnSwipeLeft += SwipedLeft;
            ClickEventsController.Instance.OnSwipeRight += SwipedRight;
        }

        private void ShowInitialClickPosition()
        {
            Debug.Log("Initial Click Position : " + ClickEventsController.Instance.GetInitialWorldPosition());
        }

        private void ShowHoldingPosition()
        {
            Debug.Log("Holding Click Position : " + ClickEventsController.Instance.GetHoldWorldPosition());
        }
        private void ShowEndPosition()
        {
            Debug.Log("End Click Position : " + ClickEventsController.Instance.GetEndWorldPosition());
        }
        private void ShowInitialToEndWorldPosition()
        {
            Debug.Log("Initial End Vector : " + ClickEventsController.Instance.GetInitialToEndWorld());
        }
        private void ShowInitialToHoldWorldDirection()
        {
            Debug.Log("Initial End Vector : " + ClickEventsController.Instance.GetInitialToHoldDirection());
        }

        private void SwipedRight()
        {
            Debug.Log("Swiped Right");
        }
        private void SwipedLeft()
        {
            Debug.Log("Swiped Left");
        }
        private void SwipedUp()
        {
            Debug.Log("Swiped Up");
        }
        private void SwipedDown()
        {
            Debug.Log("Swiped Down");
        }
    }

}