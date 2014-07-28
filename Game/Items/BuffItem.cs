///<summary>
///BuffItem.cs
///July 25,2012
///Buff items class. Each equipable item has one
///</summary>
using UnityEngine;
using System.Collections;
using System;

public class BuffItem : Item {	
	private Hashtable _buffs;
	
	public BuffItem(){
		_buffs = new Hashtable();
	}
	
	public BuffItem(Hashtable ht){
		_buffs = ht;
	}
	
	public void AddBuff(BaseStat stat, int mod){
		try{
		_buffs.Add(stat.Name,mod);
		}
		catch(Exception ex){
			Debug.LogWarning("BuffItem Class Exception" + ex);
		}
	}
	
	public void RemoveBuff(BaseStat stat){
		_buffs.Remove(stat.Name);
	}
	
	public int BuffCount(){
		return _buffs.Count;	
	}
	
	public Hashtable GetBuffs(){
		return _buffs;
	}
}
