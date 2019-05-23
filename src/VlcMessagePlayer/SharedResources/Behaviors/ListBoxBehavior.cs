using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using Vlc.DotNet.Core;

namespace VlcMessagePlayer.ShraredResources.Behaviors
{
    public class ListBoxBehavior
    {
        private static readonly Dictionary<ListBox, ListBoxBehavior.Capture> Associations = new Dictionary<ListBox, ListBoxBehavior.Capture>();
        public static readonly DependencyProperty ScrollOnNewItemProperty = DependencyProperty.RegisterAttached("ScrollOnNewItem", typeof(bool), typeof(ListBoxBehavior), (PropertyMetadata)new UIPropertyMetadata((object)false, new PropertyChangedCallback(ListBoxBehavior.OnScrollOnNewItemChanged)));

        public static bool GetScrollOnNewItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ListBoxBehavior.ScrollOnNewItemProperty);
        }

        public static void SetScrollOnNewItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ListBoxBehavior.ScrollOnNewItemProperty, (object)(int)(value ? 1 : 0));
        }

        public static void OnScrollOnNewItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox key = d as ListBox;
            if (key == null)
                return;
            bool flag1 = (bool)e.OldValue;
            bool flag2 = (bool)e.NewValue;
            if (flag2 == flag1)
                return;
            if (flag2)
            {
                key.Loaded += new RoutedEventHandler(ListBoxBehavior.ListBox_Loaded);
                key.Unloaded += new RoutedEventHandler(ListBoxBehavior.ListBox_Unloaded);
                TypeDescriptor.GetProperties((object)key)["ItemsSource"].AddValueChanged((object)key, new EventHandler(ListBoxBehavior.ListBox_ItemsSourceChanged));
            }
            else
            {
                key.Loaded -= new RoutedEventHandler(ListBoxBehavior.ListBox_Loaded);
                key.Unloaded -= new RoutedEventHandler(ListBoxBehavior.ListBox_Unloaded);
                if (ListBoxBehavior.Associations.ContainsKey(key))
                    ListBoxBehavior.Associations[key].Dispose();
                TypeDescriptor.GetProperties((object)key)["ItemsSource"].RemoveValueChanged((object)key, new EventHandler(ListBoxBehavior.ListBox_ItemsSourceChanged));
            }
        }

        private static void ListBox_ItemsSourceChanged(object sender, EventArgs e)
        {
            ListBox index = (ListBox)sender;
            if (ListBoxBehavior.Associations.ContainsKey(index))
                ListBoxBehavior.Associations[index].Dispose();
            ListBoxBehavior.Associations[index] = new ListBoxBehavior.Capture(index);
        }

        private static void ListBox_Unloaded(object sender, RoutedEventArgs e)
        {
            ListBox key = (ListBox)sender;
            if (ListBoxBehavior.Associations.ContainsKey(key))
                ListBoxBehavior.Associations[key].Dispose();
            key.Unloaded -= new RoutedEventHandler(ListBoxBehavior.ListBox_Unloaded);
        }

        private static void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            if ((INotifyCollectionChanged)listBox.Items == null)
                return;
            listBox.Loaded -= new RoutedEventHandler(ListBoxBehavior.ListBox_Loaded);
            ListBoxBehavior.Associations[listBox] = new ListBoxBehavior.Capture(listBox);
        }

        private class Capture : IDisposable
        {
            private readonly ListBox listBox;
            private readonly INotifyCollectionChanged incc;

            public Capture(ListBox listBox)
            {
                this.listBox = listBox;
                this.incc = listBox.ItemsSource as INotifyCollectionChanged;
                if (this.incc == null)
                    return;
                this.incc.CollectionChanged += new NotifyCollectionChangedEventHandler(this.incc_CollectionChanged);
            }

            private void incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action != NotifyCollectionChangedAction.Add)
                    return;
                this.listBox.ScrollIntoView(e.NewItems[0]);
                this.listBox.SelectedItem = e.NewItems[0];
            }

            public void Dispose()
            {
                if (this.incc == null)
                    return;
                this.incc.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.incc_CollectionChanged);
            }
        }
    }
}
