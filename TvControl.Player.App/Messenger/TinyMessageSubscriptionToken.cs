using System;

namespace TinyMessenger
{
    /// <summary>
    /// Represents an active subscription to a message
    /// </summary>
    public sealed class TinyMessageSubscriptionToken : IDisposable
    {
        private WeakReference _Hub;
        private Type _MessageType;

        /// <summary>
        /// Initializes a new instance of the TinyMessageSubscriptionToken class.
        /// </summary>
        public TinyMessageSubscriptionToken(ITinyMessengerHub hub, Type messageType)
        {
            if (hub == null)
                throw new ArgumentNullException("hub");

            if (!typeof(ITinyMessage).IsAssignableFrom(messageType))
                throw new ArgumentOutOfRangeException("messageType");

            this._Hub = new WeakReference(hub);
            this._MessageType = messageType;
        }

        public void Dispose()
        {
            if (this._Hub.IsAlive)
            {
                var hub = this._Hub.Target as ITinyMessengerHub;

                if (hub != null)
                {
                    var unsubscribeMethod = typeof(ITinyMessengerHub).GetMethod("Unsubscribe", new Type[] {typeof(TinyMessageSubscriptionToken)});
                    unsubscribeMethod = unsubscribeMethod.MakeGenericMethod(this._MessageType);
                    unsubscribeMethod.Invoke(hub, new object[] {this});
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}