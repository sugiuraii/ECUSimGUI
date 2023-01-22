const { app, BrowserWindow } = require('electron')
const path = require('path')

app.commandLine.appendSwitch('ignore-certificate-errors');

const createWindow = () => {
  // Create browser window
  const mainWindow = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js')
    }
  })

  return mainWindow;
}

app.whenReady().then(async () => {
  const ffp = require('find-free-ports');
  const freeport = await ffp.findFreePorts(1,{endPort : 6000, startPort : 5020});
  const mainAddr = 'http://localhost:'+ freeport + "/";
  console.log("Start server process by listening url of :"+ mainAddr);
  
  const appPath = app.getAppPath();
  const subpr= require('child_process').spawn('./ECUSimGUI',['--urls' , mainAddr], {cwd: path.join(appPath, "server-bin")});
  const axios = require('axios'); 

  app.on('before-quit', () => {
    subpr.kill('SIGINT');
  });
  
  const mainWindow = createWindow();
  
  const startup = function() {
    axios.get(mainAddr).then(function() {
      mainWindow.loadURL(mainAddr);
      
      mainWindow.on('closed', function() {
        //mainWindow = null;
      });  
    }).catch(function(err){
      //console.log(err);
      //console.log('waiting for the server start...');
      startup();
    });;
  };

  startup();
  
  app.on('activate', () => {
    // macOS では、Dock アイコンのクリック時に他に開いているウインドウがない
    // 場合、アプリのウインドウを再作成するのが一般的です。
    if (BrowserWindow.getAllWindows().length === 0) createWindow()
  });
})

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit()
})
