

class Frame {
  constructor() {

    this.scene = new THREE.Scene();

    this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1.0, 1000.0);
    this.camera.position.z = 10.0;

    this.memory = [];
    for(var idx = 0; idx < 4096; idx ++) this.memory.push(new Brush(this.memory, this.scene));


  }

  update(dt) {
    if(this.scene.children.length != 0) {
      this.scene.children.forEach((function(obj, idx) { obj.update(dt); }).bind(this));
    }
    else {
      this.scene.add(this.memory.pop().init());
    }
  }
}
