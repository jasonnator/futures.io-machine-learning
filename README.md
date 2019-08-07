# futures.io-machine-learning
This is an open source, www.futures.io member-driven repository with a focus on machine learning.

## Status
 - [x] Basic TensorFlow 2.0 beta within Docker (cpu)
 - [ ] Basic TensorFlow 2.0 beta within Docker (gpu)
	 - [ ] Detailed gpu how to wiki page
 - [x] Basic TensorFlow Serving within Docker
	- [x] Mount host directory into docker container
	- [x] Unit test receive and parse prediction
	- [ ] Container discover new saved models
 - [x] Proof of concept call TensorFlow from within NT8
	- [x] Via WebRequest (default port 8501), see `_TensorFlowStrategyTest.cs`
	- [ ] Via gRPC (default port 8500)
 - [ ] Basic TensorBoard dashboard 

## How To Documents
 - [TensorFlow Docker How To]

[TensorFlow Docker How To]: https://github.com/jasonnator/futures.io-machine-learning/wiki/TensorFlow-Docker-How-To
[TensorFlow Docker Official]: https://www.tensorflow.org/install/docker
[DockerHub TensorFlow]: https://hub.docker.com/r/tensorflow/tensorflow/tags?page=3