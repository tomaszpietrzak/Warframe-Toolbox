pipeline {
  agent any
  
	node {
		stage 'Checkout'
			checkout scm

		stage 'Build'
			bat 'nuget restore Warframe Toolbox.sln'
			bat "\"${tool 'MSBuild'}\" Warframe Toolbox.sln /p:Configuration=Release"

		stage 'Archive'
			archive 'ProjectName/bin/Release/**'

	}
}
