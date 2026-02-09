using UnityEngine;

namespace Sean.Combat
{
    public class DirectionIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject arrowUp;
        [SerializeField] private GameObject arrowDown;
        [SerializeField] private GameObject arrowLeft;
        [SerializeField] private GameObject arrowRight;

        private void Awake()
        {
            HideAll();
        }

        public void ShowDirection(AttackDirection direction)
        {
            HideAll();

            switch (direction)
            {
                case AttackDirection.Up:
                    arrowUp.SetActive(true);
                    break;
                case AttackDirection.Down:
                    arrowDown.SetActive(true);
                    break;
                case AttackDirection.Left:
                    arrowLeft.SetActive(true);
                    break;
                case AttackDirection.Right:
                    arrowRight.SetActive(true);
                    break;
            }
        }

        public void HideAll()
        {
            arrowUp.SetActive(false);
            arrowDown.SetActive(false);
            arrowLeft.SetActive(false);
            arrowRight.SetActive(false);
        }
    }
}
