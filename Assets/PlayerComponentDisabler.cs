using Mirror;
using UnityEngine;

public class PlayerComponentDisabler : NetworkBehaviour
{
    [SerializeField] private Behaviour[] _componentsToDisable;
    [SerializeField] private GameObject[] _goToDisable;

    private void Start()
    {
        if (hasAuthority)
            return;

        if(_componentsToDisable != null && _componentsToDisable.Length > 0)
            foreach (var bh in _componentsToDisable)
                bh.enabled = false;

        if(_goToDisable != null && _goToDisable.Length > 0)
            foreach (var go in _goToDisable)
                go.SetActive(false);
    }
}
