if(-not (test-path TeamConfig\bin\Debug\TeamConfig.exe)) {
  msbuild
}

& TeamConfig\bin\Debug\TeamConfig.exe Sample\Config Sample\Templates\Thing.config.template Sample\

invoke-item Sample