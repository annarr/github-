﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Emgu.CV;
using SAD.Core.Devices;
using SAD.Core.IO;

namespace SADGUI
{
    class MainViewModel : ViewModelBase
    {
        private BitmapSource m_cameraImage;
        private Capture m_capture;
        private IMissileLauncher m_missileLauncher;
        private int move;
        private string m_missileCount;

        public MainViewModel()
        {
            GetImageCommand = new MyCommands(GetImage);
            LoadTargetsFromFileCommand = new MyCommands(LoadTargetsFromFile);
            MoveRightCommand = new MyCommands(MoveRight);
            MoveLeftCommand = new MyCommands(MoveLeft);
            MoveUpCommand = new MyCommands(MoveUp);
            MoveDownCommand = new MyCommands(MoveDown);
            FireCommand = new MyCommands(Fire);
            move = 5;
        }

        private void GetImage()
        {
            if (m_capture == null)
            {
                m_capture = new Capture(0);
            }
            var image = m_capture.QueryFrame();
            image.Save(@"c:\testfolder\test.png");
            var wpfImage = ConvertImageToBitmap(image);

            CameraImage = wpfImage;

        }
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr ptr);
        private static BitmapSource ConvertImageToBitmap(IImage image)
        {
            if (image != null)
            {
                using (var source = image.Bitmap)
                {
                    var hbitmap = source.GetHbitmap();
                    try
                    {
                        var bitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero,
                                                     Int32Rect.Empty,
                                                     BitmapSizeOptions.FromEmptyOptions());
                        DeleteObject(hbitmap);
                        bitmap.Freeze();
                        return bitmap;
                    }
                    catch
                    {
                        image = null;
                    }
                }
            }
            return null;
        }
        public BitmapSource CameraImage
        {
            get { return m_cameraImage; }
            set
            {
                if (Equals(value, m_cameraImage))
                {
                    return;
                }
                m_cameraImage = value;
                OnPropertyChanged();
            }
        }
        public string MissileCount
        {
            get { return m_missileCount; }
            set
            {
                m_missileCount = value;
                OnPropertyChanged();
            }
        }

        private void GetCount()
        {
            string propertyName = "MissileCount";
            Object MC;
            string SMC;

            MC = m_missileLauncher.GetType().GetProperty(propertyName).GetValue(m_missileLauncher, null);
            SMC = MC.ToString();
            MissileCount = SMC;
        }
        public ICommand LoadTargetsFromFileCommand { get; set; }
        public ICommand GetImageCommand{ get; set; }
        public ICommand MoveRightCommand { get; set; }
        public ICommand MoveLeftCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }
        public ICommand FireCommand { get; set; }
        private void LoadTargetsFromFile()
        {
            var openFileDialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            var worked = openFileDialog.ShowDialog();
            if (worked == true)
            {
                MessageBox.Show("We loaded: " + openFileDialog.FileName);
                var iniReader = FRFactory.CreateReader(FRType.INIReader, openFileDialog.FileName);
            }
        }

        private void MoveRight()
        {
            if (!(m_missileLauncher is DreamCheeky)) //if m_missilelauncher is not a DreamCheeky
            {                                        //set it to a new DreamCheeky
                m_missileLauncher = MLFactory.CreateMissileLauncher(MLType.DreamCheeky);
            }
            //if it is a DreamCheeky, or the above statement executed move 5 degrees to the right
            //m_missileLauncher.MoveTo(0,0);
            m_missileLauncher.MoveBy((0), (move * 22.2));
                
        }
        private void MoveLeft()
        {
            if (!(m_missileLauncher is DreamCheeky)) //if m_missilelauncher is not a DreamCheeky
            {                                        //set it to a new DreamCheeky
                m_missileLauncher = MLFactory.CreateMissileLauncher(MLType.DreamCheeky);
            }
            //if it is a DreamCheeky, or the above statement executed move 5 degrees to the Left
            //m_missileLauncher.MoveTo(0,0);
            m_missileLauncher.MoveBy((0), ((move*-1) * 22.2));

        }
        private void MoveUp()
        {
            if (!(m_missileLauncher is DreamCheeky)) //if m_missilelauncher is not a DreamCheeky
            {                                        //set it to a new DreamCheeky
                m_missileLauncher = MLFactory.CreateMissileLauncher(MLType.DreamCheeky);
            }
            //if it is a DreamCheeky, or the above statement executed move 5 degrees to the up
            //m_missileLauncher.MoveTo(0,0);
            m_missileLauncher.MoveBy((move * 22.2), (0));

        }
        private void MoveDown()
        {
            if (!(m_missileLauncher is DreamCheeky)) //if m_missilelauncher is not a DreamCheeky
            {                                        //set it to a new DreamCheeky
                m_missileLauncher = MLFactory.CreateMissileLauncher(MLType.DreamCheeky);
            }
            //if it is a DreamCheeky, or the above statement executed move 5 degrees to the down
            //m_missileLauncher.MoveTo(0,0);
            m_missileLauncher.MoveBy(((move*-1) * 22.2), (0));

        }
        private void Fire()
        {
            if (!(m_missileLauncher is DreamCheeky)) //if m_missilelauncher is not a DreamCheeky
            {                                        //set it to a new DreamCheeky
                m_missileLauncher = MLFactory.CreateMissileLauncher(MLType.DreamCheeky);
            }
            //if it is a DreamCheeky, or the above statement executed Fire a missile
            //m_missileLauncher.MoveTo(0,0);
            m_missileLauncher.Fire();
            GetCount();

        }
    }

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
