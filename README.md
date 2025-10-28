# AnimatorSync
UPM Package for syncing Animators between screens. Supports PUN2 and (maybe) Fusion.


The package exposes a component AnimatorSyncPUN (for PUN 2) and AnimatorSyncFusion (for Fusion).

On the owning client the component reads Animator parameters and triggers and serializes them over the network.

On remote clients it receives updates and applies them to the local Animator with smoothing/interpolation for float parameters and immediate application for ints/bools/triggers (triggers can be buffered and optionally replayed for late joiners).

Triggers are handled via reliable RPC messages (PUN RPCs and Fusion RPC/Buffered events) because triggers are one-shot.

The component is configurable: which parameters to sync, per-parameter thresholds, send rate, reliable/unreliable for bandwidth control, and owner-only writing.
