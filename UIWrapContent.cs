﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020005A0 RID: 1440
[AddComponentMenu("NGUI/Interaction/Wrap Content")]
public class UIWrapContent : MonoBehaviour
{
	// Token: 0x0600283D RID: 10301 RVA: 0x00128A74 File Offset: 0x00126E74
	protected virtual void Start()
	{
		this.SortBasedOnScrollMovement();
		this.WrapContent();
		if (this.mScroll != null)
		{
			this.mScroll.GetComponent<UIPanel>().onClipMove = new UIPanel.OnClippingMoved(this.OnMove);
		}
		this.mFirstTime = false;
	}

	// Token: 0x0600283E RID: 10302 RVA: 0x00128AC2 File Offset: 0x00126EC2
	protected virtual void OnMove(UIPanel panel)
	{
		this.WrapContent();
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x00128ACC File Offset: 0x00126ECC
	[ContextMenu("Sort Based on Scroll Movement")]
	public virtual void SortBasedOnScrollMovement()
	{
		if (!this.CacheScrollView())
		{
			return;
		}
		this.mChildren.Clear();
		for (int i = 0; i < this.mTrans.childCount; i++)
		{
			Transform child = this.mTrans.GetChild(i);
			if (!this.hideInactive || child.gameObject.activeInHierarchy)
			{
				this.mChildren.Add(child);
			}
		}
		if (this.mHorizontal)
		{
			List<Transform> list = this.mChildren;
			if (UIWrapContent.<>f__mg$cache0 == null)
			{
				UIWrapContent.<>f__mg$cache0 = new Comparison<Transform>(UIGrid.SortHorizontal);
			}
			list.Sort(UIWrapContent.<>f__mg$cache0);
		}
		else
		{
			List<Transform> list2 = this.mChildren;
			if (UIWrapContent.<>f__mg$cache1 == null)
			{
				UIWrapContent.<>f__mg$cache1 = new Comparison<Transform>(UIGrid.SortVertical);
			}
			list2.Sort(UIWrapContent.<>f__mg$cache1);
		}
		this.ResetChildPositions();
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x00128BAC File Offset: 0x00126FAC
	[ContextMenu("Sort Alphabetically")]
	public virtual void SortAlphabetically()
	{
		if (!this.CacheScrollView())
		{
			return;
		}
		this.mChildren.Clear();
		for (int i = 0; i < this.mTrans.childCount; i++)
		{
			Transform child = this.mTrans.GetChild(i);
			if (!this.hideInactive || child.gameObject.activeInHierarchy)
			{
				this.mChildren.Add(child);
			}
		}
		List<Transform> list = this.mChildren;
		if (UIWrapContent.<>f__mg$cache2 == null)
		{
			UIWrapContent.<>f__mg$cache2 = new Comparison<Transform>(UIGrid.SortByName);
		}
		list.Sort(UIWrapContent.<>f__mg$cache2);
		this.ResetChildPositions();
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x00128C54 File Offset: 0x00127054
	protected bool CacheScrollView()
	{
		this.mTrans = base.transform;
		this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		this.mScroll = this.mPanel.GetComponent<UIScrollView>();
		if (this.mScroll == null)
		{
			return false;
		}
		if (this.mScroll.movement == UIScrollView.Movement.Horizontal)
		{
			this.mHorizontal = true;
		}
		else
		{
			if (this.mScroll.movement != UIScrollView.Movement.Vertical)
			{
				return false;
			}
			this.mHorizontal = false;
		}
		return true;
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x00128CE0 File Offset: 0x001270E0
	protected virtual void ResetChildPositions()
	{
		int i = 0;
		int count = this.mChildren.Count;
		while (i < count)
		{
			Transform transform = this.mChildren[i];
			transform.localPosition = ((!this.mHorizontal) ? new Vector3(0f, (float)(-(float)i * this.itemSize), 0f) : new Vector3((float)(i * this.itemSize), 0f, 0f));
			this.UpdateItem(transform, i);
			i++;
		}
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x00128D68 File Offset: 0x00127168
	public virtual void WrapContent()
	{
		float num = (float)(this.itemSize * this.mChildren.Count) * 0.5f;
		Vector3[] worldCorners = this.mPanel.worldCorners;
		for (int i = 0; i < 4; i++)
		{
			Vector3 vector = worldCorners[i];
			vector = this.mTrans.InverseTransformPoint(vector);
			worldCorners[i] = vector;
		}
		Vector3 vector2 = Vector3.Lerp(worldCorners[0], worldCorners[2], 0.5f);
		bool flag = true;
		float num2 = num * 2f;
		if (this.mHorizontal)
		{
			float num3 = worldCorners[0].x - (float)this.itemSize;
			float num4 = worldCorners[2].x + (float)this.itemSize;
			int j = 0;
			int count = this.mChildren.Count;
			while (j < count)
			{
				Transform transform = this.mChildren[j];
				float num5 = transform.localPosition.x - vector2.x;
				if (num5 < -num)
				{
					Vector3 localPosition = transform.localPosition;
					localPosition.x += num2;
					num5 = localPosition.x - vector2.x;
					int num6 = Mathf.RoundToInt(localPosition.x / (float)this.itemSize);
					if (this.minIndex == this.maxIndex || (this.minIndex <= num6 && num6 <= this.maxIndex))
					{
						transform.localPosition = localPosition;
						this.UpdateItem(transform, j);
					}
					else
					{
						flag = false;
					}
				}
				else if (num5 > num)
				{
					Vector3 localPosition2 = transform.localPosition;
					localPosition2.x -= num2;
					num5 = localPosition2.x - vector2.x;
					int num7 = Mathf.RoundToInt(localPosition2.x / (float)this.itemSize);
					if (this.minIndex == this.maxIndex || (this.minIndex <= num7 && num7 <= this.maxIndex))
					{
						transform.localPosition = localPosition2;
						this.UpdateItem(transform, j);
					}
					else
					{
						flag = false;
					}
				}
				else if (this.mFirstTime)
				{
					this.UpdateItem(transform, j);
				}
				if (this.cullContent)
				{
					num5 += this.mPanel.clipOffset.x - this.mTrans.localPosition.x;
					if (!UICamera.IsPressed(transform.gameObject))
					{
						NGUITools.SetActive(transform.gameObject, num5 > num3 && num5 < num4, false);
					}
				}
				j++;
			}
		}
		else
		{
			float num8 = worldCorners[0].y - (float)this.itemSize;
			float num9 = worldCorners[2].y + (float)this.itemSize;
			int k = 0;
			int count2 = this.mChildren.Count;
			while (k < count2)
			{
				Transform transform2 = this.mChildren[k];
				float num10 = transform2.localPosition.y - vector2.y;
				if (num10 < -num)
				{
					Vector3 localPosition3 = transform2.localPosition;
					localPosition3.y += num2;
					num10 = localPosition3.y - vector2.y;
					int num11 = Mathf.RoundToInt(localPosition3.y / (float)this.itemSize);
					if (this.minIndex == this.maxIndex || (this.minIndex <= num11 && num11 <= this.maxIndex))
					{
						transform2.localPosition = localPosition3;
						this.UpdateItem(transform2, k);
					}
					else
					{
						flag = false;
					}
				}
				else if (num10 > num)
				{
					Vector3 localPosition4 = transform2.localPosition;
					localPosition4.y -= num2;
					num10 = localPosition4.y - vector2.y;
					int num12 = Mathf.RoundToInt(localPosition4.y / (float)this.itemSize);
					if (this.minIndex == this.maxIndex || (this.minIndex <= num12 && num12 <= this.maxIndex))
					{
						transform2.localPosition = localPosition4;
						this.UpdateItem(transform2, k);
					}
					else
					{
						flag = false;
					}
				}
				else if (this.mFirstTime)
				{
					this.UpdateItem(transform2, k);
				}
				if (this.cullContent)
				{
					num10 += this.mPanel.clipOffset.y - this.mTrans.localPosition.y;
					if (!UICamera.IsPressed(transform2.gameObject))
					{
						NGUITools.SetActive(transform2.gameObject, num10 > num8 && num10 < num9, false);
					}
				}
				k++;
			}
		}
		this.mScroll.restrictWithinPanel = !flag;
		this.mScroll.InvalidateBounds();
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x0012926B File Offset: 0x0012766B
	private void OnValidate()
	{
		if (this.maxIndex < this.minIndex)
		{
			this.maxIndex = this.minIndex;
		}
		if (this.minIndex > this.maxIndex)
		{
			this.maxIndex = this.minIndex;
		}
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x001292A8 File Offset: 0x001276A8
	protected virtual void UpdateItem(Transform item, int index)
	{
		if (this.onInitializeItem != null)
		{
			int realIndex = (this.mScroll.movement != UIScrollView.Movement.Vertical) ? Mathf.RoundToInt(item.localPosition.x / (float)this.itemSize) : Mathf.RoundToInt(item.localPosition.y / (float)this.itemSize);
			this.onInitializeItem(item.gameObject, index, realIndex);
		}
	}

	// Token: 0x04002910 RID: 10512
	public int itemSize = 100;

	// Token: 0x04002911 RID: 10513
	public bool cullContent = true;

	// Token: 0x04002912 RID: 10514
	public int minIndex;

	// Token: 0x04002913 RID: 10515
	public int maxIndex;

	// Token: 0x04002914 RID: 10516
	public bool hideInactive;

	// Token: 0x04002915 RID: 10517
	public UIWrapContent.OnInitializeItem onInitializeItem;

	// Token: 0x04002916 RID: 10518
	protected Transform mTrans;

	// Token: 0x04002917 RID: 10519
	protected UIPanel mPanel;

	// Token: 0x04002918 RID: 10520
	protected UIScrollView mScroll;

	// Token: 0x04002919 RID: 10521
	protected bool mHorizontal;

	// Token: 0x0400291A RID: 10522
	protected bool mFirstTime = true;

	// Token: 0x0400291B RID: 10523
	protected List<Transform> mChildren = new List<Transform>();

	// Token: 0x0400291C RID: 10524
	[CompilerGenerated]
	private static Comparison<Transform> <>f__mg$cache0;

	// Token: 0x0400291D RID: 10525
	[CompilerGenerated]
	private static Comparison<Transform> <>f__mg$cache1;

	// Token: 0x0400291E RID: 10526
	[CompilerGenerated]
	private static Comparison<Transform> <>f__mg$cache2;

	// Token: 0x020005A1 RID: 1441
	// (Invoke) Token: 0x06002847 RID: 10311
	public delegate void OnInitializeItem(GameObject go, int wrapIndex, int realIndex);
}
