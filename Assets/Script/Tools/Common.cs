using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Plugin;

[Serializable]
public class MovePacket
{
    public TEAM team = TEAM.Neutral;
	public string from;
	public string to;
	public List<string> tags = new List<string>();
	public float rate;
}

[Serializable]
public class SkillPacket
{
	public int skillID;
	public TEAM from;
	public TEAM to;
	public string tag;
}

[Serializable]
public class UnknownSkillPacket
{
    public int skillID;
    public TEAM from;
    public TEAM to;
    public string tag;
    public string transformId;
}

[Serializable]
public class GiveUpPacket
{
	public TEAM team;
}

[Serializable]
public class PlanetBomb
{
    public string tag;
}

[Serializable]
public class DriftEffect
{
    public string tag;
    public string effect;
    public float time;
    public float scale;
}

[Serializable]
public class PlanetAttack
{
    public string tag;
}

[Serializable]
public class AddAttack
{
    public string tag;
}

[Serializable]
public class GunturretAttack
{
    public string tag;
}

[Serializable]
public class LasergunAttack
{
    public string tag;
}

[Serializable]
public class TwistAttack
{
    public string tag;
}

[Serializable]
public class PlanetTeam
{
    public string tag;
    public TEAM t;
}

[Serializable]
public class FramePacket
{
	public byte type;
	public MovePacket move;
	public SkillPacket skill;
    public UnknownSkillPacket unknown;
	public GiveUpPacket giveup;
    public PlanetBomb bomb;
    public DriftEffect effect;
    public PlanetAttack attack;
    public AddAttack addattack;
    public GunturretAttack gunattack;
    public LasergunAttack laserattack;
    public TwistAttack twistattack;
    public PlanetTeam team;
}

public class Packet
{
	public TEAM team;
	public FramePacket packet;
}

public class ChessItem
{
	public static int CHESS_MAX = 5;
	public int id{ get; set;}
	public Int64 timeout{ get; set;}
	public DateTime timefinish{ get; set;}
	public int slot{ get; set;}
}

public enum TEAM : byte
{
	Neutral,
	Team_1,
	Team_2,
	Team_3,
	Team_4,
	Team_5,
	Team_6,

	Team_Black1,	// 黑暗势力1
	Team_Black2,	// 黑暗势力2

	TeamMax,
}

/// <summary>
/// Vector 数学运算.
/// </summary>
public static class VectorMath
{
	/// <summary>
	/// Angles the axis.
	/// </summary>
	/// <returns>The axis.</returns>
	/// <param name="axis">Axis.</param>
	/// <param name="angle">Angle.</param>
	static public Quaternion AngleAxis(float angle, Vector3 axis)
	{
		float mag = axis.magnitude;

		if (mag > 0.000001F) {
			float halfAngle = angle * 0.5F;

			Quaternion q = new Quaternion ();
			q.w = Mathf.Cos (halfAngle);

			float s = Mathf.Sin (halfAngle) / mag;
			q.x = s * axis.x;
			q.y = s * axis.y;
			q.z = s * axis.z;

			return q;
		} else {
			return Quaternion.identity;
		}

	}

	/// <summary>
	/// 围绕某点旋转, 角度的单位为弧度
	/// </summary>
	/// <returns>The around.</returns>
	/// <param name="point">Point.</param>
	/// <param name="axis">Axis.</param>
	/// <param name="angle">Angle.</param>
	static public Vector3 RotateAround(this Vector3 position, Vector3 point, Vector3 axis, float angle)
	{
		Vector3 vector = position;
		Quaternion rotation = VectorMath.AngleAxis(angle, axis);
		Vector3 vector2 = vector - point;
		vector2 = rotation * vector2;
		vector = point + vector2;
		position = vector;

		return position;
	}

	/// <summary>
	/// 旋转呀
	/// </summary>
	/// <param name="quaternion">Quaternion.</param>
	/// <param name="eulerAngles">Euler angles.</param>
	static public Quaternion Rotate(this Quaternion quaternion, Vector3 eulerAngles)
	{ 
		Quaternion rhs = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);

		return quaternion *= rhs;
	}
}