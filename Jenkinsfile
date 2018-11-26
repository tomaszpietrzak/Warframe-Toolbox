pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh '''bat \'nuget restore Warframe Toolbox.sln\'

bat "\\"${tool \'msbuild\'}\\" Warframe Toolbox.sln /p:Configuration=Debug"'''
      }
    }
  }
}