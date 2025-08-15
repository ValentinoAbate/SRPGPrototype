using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableItemHandler
{
    public bool TrySelectItem(object item);
}
