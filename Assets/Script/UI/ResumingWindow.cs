﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Solarmax;

public class ResumingWindow : BaseWindow {

	public UILabel tips;

	private float waitBeginTime;

	public override bool Init ()
	{
        base.Init();
        return true;
	}

	public override void OnShow ()
	{
        base.OnShow();
    }

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
		
	}

	public override void OnHide ()
	{

	}
}
