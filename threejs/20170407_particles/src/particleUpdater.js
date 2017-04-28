/* particles */
/*
  파티클 업데이터 {

파티클 위치 texture
파티클 속도 texture

파티클 랜더링

}

*/

class ParticleUpdater {
  constructor(cnt, rdrr) {
    this.elapsedtime = 0.0;
    this.pivot = new THREE.Vector3(0.0, 0.0, 0.0);

    this.rdrr = rdrr;
    this.scene = new THREE.Scene();
    this.camera = new THREE.PerspectiveCamera(60, window.innerWidth / window.innerHeight, 1.0, 1000.0);
    this.camera.position.z = 500.0 * Math.cos(0.0);
    this.camera.lookAt(this.pivot);

    // this.camera.position.x = 500.0 * Math.sin(0.0);
    // this.camera.position.x = 500.0 * Math.cos(0.5);
    // this.camera.lookAt(new THREE.Vector3(0.0, 0.0, 0.0));

    this.position = new ParticlePosition(cnt, rdrr);
    this.velocity = new ParticleVelocity(cnt, rdrr);
    this.life = new ParticleLife(cnt, rdrr);

    this.particles = new Particles(this.position, this.velocity, this.life, cnt);

    this.scene.add(this.particles);

    document.body.addEventListener("mousemove", ({pageX, pageY})=> {
      this.particles._position.setCreatePoint(
        pageX / window.innerWidth * 100.0 - 50.0,
        50.0 - pageY / window.innerHeight * 100.0);
    })
  }

  update(dt) {
    if(isNaN(dt) == false) this.elapsedtime += dt;
    // this.camera.position.x = 500.0 * Math.cos( Math.PI * 0.1);
    this.camera.position.z = 400.0 ;//Math.sin( Math.PI * 0.1);
    // this.camera.position.y = 500.0 * Math.sin( Math.PI * 0.4);
    this.camera.lookAt(this.pivot);

    this.life.update(dt);
    this.velocity.update();
    this.position.update(dt, this.velocity, this.life);
  }

  render() {
    this.rdrr.render(this.scene, this.camera);
  }

};
