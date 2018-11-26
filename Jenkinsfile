pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh '''bat \'nuget restore Warframe Toolbox.sln\'

bat "\\"${tool \'MSBuild\'}\\" Warframe Toolbox.sln /p:Configuration=Release"'''
      }
    }
  }
}