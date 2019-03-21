# kappa-recipe
Provide a recipe for writing kappa architecture microservices in .Net core

The whole architecture revolves around the concept of a microservice that has options as to its behaviour upon startup.
For a stateful microservice, the capability exists to define the startup behaviour through the 
SetStartupBehaviour of the DataflowBuilder. If that is set to StartAtLastSnapshot then the microservice will invoke the SnapshotRetriever delegate. If no Snapshot is found (as would be the case on the 1st run), then the SnapshotInitializer delegate is called.

The resulting microservice is basically a concrete implementation of IDataflow.  I am providing a default concrete implementation in SyncDataflow. DataflowBuilder constructs the IDataflow that is passed into the Initialize method.

The whole service is basically a loop that calls the InputGetter delegate, then applies to the input to its state by calling the StateRetriever and Processor and then finally stores its state by calling the StateSetter.

If the StatePublisher delegate is set, the state will also be published.
If the SnapshotUpdater delegate is set, it will be called when the SnapshotTimer indicates that it is appropriate to snapshot.


