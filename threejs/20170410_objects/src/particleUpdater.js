

class ParticleUpdater {
  constructor(cnt, rdrr) {
    this.rdrr = rdrr;
    this.position = new ParticlePosition(cnt, rdrr);
    this.velocity = new ParticleVelocity(cnt, rdrr);
  }

  update(dt) {
    this.velocity.update();
    this.position.update(dt, this.velocity);
  }

  render() {

  }

};
