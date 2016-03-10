using UnityEngine;
using System.Collections.Generic;

public class StoreBridgeBase  {
	protected static Notify notify;

	public delegate void ProductListReceivedHandler(  Dictionary<string, IAP_DATA> iapTable);
	protected static event ProductListReceivedHandler onProductListReceivedEvent = null;	
	public void RegisterForProductListReceived( ProductListReceivedHandler delg) {
		onProductListReceivedEvent += delg; }	
	public void UnregisterForProductListReceived( ProductListReceivedHandler delg) {
		onProductListReceivedEvent -= delg; }

	public delegate void ProductListFailedHandler(string error, bool isErrorLocalized  );
	protected static event ProductListFailedHandler onProductListFailedEvent = null;	
	public void RegisterForProductListFailed( ProductListFailedHandler delg) {
		onProductListFailedEvent += delg; }	
	public void UnregisterForProductListFailed( ProductListFailedHandler delg) {
		onProductListFailedEvent -= delg; }
	
	public delegate void PurchaseSuccessfulHandler(string productIdentifier );
	protected static event PurchaseSuccessfulHandler onPurchaseSuccessfulEvent = null;	
	public void RegisterForPurchaseSuccessful( PurchaseSuccessfulHandler delg) {
		onPurchaseSuccessfulEvent += delg; }	
	public void UnregisterForPurchaseSuccessful( PurchaseSuccessfulHandler delg) {
		onPurchaseSuccessfulEvent -= delg; }	
	
	
	public delegate void PurchaseFailedHandler(string error , bool isErrorLocalized );
	protected static event PurchaseFailedHandler onPurchaseFailedEvent = null;	
	public void RegisterForPurchaseFailed( PurchaseFailedHandler delg) {
		onPurchaseFailedEvent += delg; }	
	public void UnregisterForPurchaseFailed( PurchaseFailedHandler delg) {
		onPurchaseFailedEvent -= delg; }	
	
	
	public delegate void PurchaseCancelledHandler(string error, bool isErrorLocalized  );
	protected static event PurchaseCancelledHandler onPurchaseCancelledEvent = null;	
	public void RegisterForPurchaseCancelled( PurchaseCancelledHandler delg) {
		onPurchaseCancelledEvent += delg; }	
	public void UnregisterForPurchaseCancelled( PurchaseCancelledHandler delg) {
		onPurchaseCancelledEvent -= delg; }			
	
	
	public delegate void LatePurchaseSuccessfulHandler(string productIdentifier );
	protected static event LatePurchaseSuccessfulHandler onLatePurchaseSuccessfulEvent = null;	
	public void RegisterForLatePurchaseSuccessful( LatePurchaseSuccessfulHandler delg) {
		onLatePurchaseSuccessfulEvent += delg; }	
	public void UnregisterForLatePurchaseSuccessful( LatePurchaseSuccessfulHandler delg) {
		onLatePurchaseSuccessfulEvent -= delg; }
	
	
	public delegate void LatePurchaseFailedHandler(string error, bool isErrorLocalized  );
	protected static event LatePurchaseFailedHandler onLatePurchaseFailedEvent = null;	
	public void RegisterForLatePurchaseFailed( LatePurchaseFailedHandler delg) {
		onLatePurchaseFailedEvent += delg; }	
	public void UnregisterForLatePurchaseFailed( LatePurchaseFailedHandler delg) {
		onLatePurchaseFailedEvent -= delg; }
	
	protected Dictionary<string, IAP_DATA> iapTable;
	
	public StoreBridgeBase()
	{
		notify = new Notify(this.GetType().Name);	
	}
	
	/// <summary>
	/// Requests the product list. assumes you've previously called RegisterForProductListReceived and RegisterForProductFailed
	/// </summary>
	public virtual void RequestProductList( string [] productIdentifiers, ref Dictionary<string, IAP_DATA> iapTableParam)
	{
		this.iapTable = iapTableParam;
		// dummy implementation just says successful
		if (onProductListReceivedEvent != null)
		{
			onProductListReceivedEvent(iapTable);	
		}
	}
	
	/// <summary>
	/// Make us listen to the native storekit / google play events
	/// </summary>
	public virtual void HookEvents()
	{
		// nothing in dummy implementation
	}
	
	public virtual void UnhookEvents()
	{
		// nothing in dummy implementation
	}
	
	public virtual void PurchaseProduct ( string productId)
	{
		// it's always successful! 
		if (onPurchaseSuccessfulEvent != null)
		{
			onPurchaseSuccessfulEvent( productId);
		}
	}
	
	protected void TriggerProductListFailed(string error, bool isErrorLocalized)
	{
		if (onProductListFailedEvent != null)
		{
			onProductListFailedEvent(error,  isErrorLocalized);	
		}		
	}
	
	protected void TriggerProductListSucceeded(Dictionary<string, IAP_DATA> iapTable)
	{
		if (onProductListReceivedEvent != null)
		{
			onProductListReceivedEvent(iapTable);
		}
	}
	
	protected void TriggerPurchaseSuccessful( string productId)
	{
		if (onPurchaseSuccessfulEvent != null)
		{
			onPurchaseSuccessfulEvent( productId);
		}		
	}
	
	protected void TriggerPurchaseFailed( string error, bool isErrorLocalized)
	{
		if (onPurchaseFailedEvent != null)
		{
			onPurchaseFailedEvent( error,isErrorLocalized);
		}
	}
	
	protected void TriggerPurchaseCancelled( string error, bool isErrorLocalized)
	{
		if (onPurchaseCancelledEvent != null)
		{
			onPurchaseCancelledEvent( error, isErrorLocalized);
		}
	}
	
	protected void TriggerLatePurchaseSuccessful( string productId)
	{
		if (onLatePurchaseSuccessfulEvent != null)
		{
			onLatePurchaseSuccessfulEvent( productId);
		}		
	}
	
	protected void TriggerLatePurchaseFailed( string error, bool isErrorLocalized)
	{
		if (onLatePurchaseFailedEvent != null)
		{
			onLatePurchaseFailedEvent( error, isErrorLocalized);
		}
	}
	

}
