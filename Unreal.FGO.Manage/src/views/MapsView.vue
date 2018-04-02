<template>
  <div class="map-warp" v-show="!$parent.loading">
    <div class="war-list">
      <div @click="spotList(war)" v-for="war in data.wars" class="war-item" v-show="showWar">
        <div class="war-header">
          <span class="left">{{war.name}}</span>
          <span class="right">{{war.age}}</span>
        </div>
        <p class="long-name">{{war.longName}}</p>
      </div>
    </div>
    <div class="spot-list" @ v-show="showSpot">
      <a class="back" @click="warList()">
        <div class="lt"></div>返回</a>
      <p style="position: fixed;bottom: 5px;left: 50%;color: #949494;font-size: 14px;text-shadow: 0px 0px 8px #b3b2b2;">鼠标滚动缩放地图</p>
<div class="spot-warp" v-for="spot in spots">
  <div class="spot-item" :class="{current : currentSpot == spot}" @click="questList(spot)" :style="{left : (spot.x - spotMinX) * zoom + 'px',top : (spot.y - spotMinY) * zoom + 'px'}">{{spot.name}}</div>
  <div class="spot-road" :style="roadStyle(spot, zoom)">
    <div class="lt"></div>
  </div>
</div>
</div>
<div class="quest-list" v-show="showQuest">
  <div style="text-align: center;font-size: 24px;background: rgb(55, 200, 247);color: rgb(255, 255, 255);position: relative;height: 68px;width: 100%;">
<a style="position: absolute;right: 6px;" href="javascript:;" @click="closeQuest">×</a>
<span style="line-height: 68px;">{{spotName}}</span>
</div>

<div class="quest-warp">
  <div class="quest-item" @click="battle(quest)" v-for="quest in quests" :class="{current : currentQuest == quest}">
    <div class="quest-head">{{quest.name}}</div>
    <div class="quest-body">
      <div class="quest-phase gt" v-for="phase in quest.phases" :class="isClear(quest, phase.phase)">
      </div>
      <span>{{questType(quest.type)}}</span>
      <span>AP:{{quest.actConsume}}</span>
    </div>
  </div>
</div>
</div>
</div>
</template>
<script>
  export default {
    name: 'maps-view',
    data() {
      return {
        currentSpot: null,
        currentQuest: null,
        data: { wars: [] },
        fire: null,
        zoom: 0.6,
        spots: [],
        spotName: "Quest",
        quests: [],
        showWar: true,
        showSpot: false,
        showQuest: false,
        spotMinX: 0,
        spotMinY: 0
      }
    },
    methods: {
      battle(quest) {
        if (this.$route.params.id) {
          this.$router.push({ path: '/battle/' + this.$route.params.id + '/' + quest.id })
        }
      },
      nextQuest() {
        for (var i = this.data.wars.length - 1; i >= 0; i--) {
          var war = this.data.wars[i]

          for (var j = war.spots.length - 1; j >= 0; j--) {
            var spot = war.spots[j]
            var quests = spot.quests.filter(q => q.type == '1')
            if (quests.length > 0) {
              this.currentSpot = spot
              this.currentQuest = quests[quests.length - 1]
              this.spotList(war)
              this.questList(spot)
              return
            }
          }
        }
      },
      questType(type) {
        switch (type) {
          case '1':
            return 'MAIN'
          case '2':
            return 'FREE'
          case '3':
            return 'FRIENDSHIP'
          case '5':
            return 'EVENT'
          case '6':
            return 'HEROBALLAD'
        }
      },
      isClear(quest, phase) {
        return quest.phase >= parseInt(phase) ? 'clear' : ''
      },
      closeQuest() {
        this.showQuest = false
      },
      questList(spot) {
        this.spotName = spot.name
        this.quests = spot.quests
        this.showQuest = true
        this.showWar = false
        this.showSpot = true
      },
      warList() {
        this.showWar = true
        this.showQuest = false
        this.showSpot = false
      },
      roadStyle(spot, zoom) {
        if (!spot || !this.data.wars)
          return ''

        var war = this.data.wars.find(w => w.id == spot.warId)
        if (!war)
          return ''
        var spotRoad = war.spotRoads.find(r => r.dstSpotId == spot.id)
        if (spotRoad) {
          var srcSpot = war.spots.find(s => s.id == spotRoad.srcSpotId)
          if (!srcSpot)
            return ''
          var dx = (spot.x - this.spotMinX) * zoom + (spot.name.length * 12 + 20) / 2, sx = (srcSpot.x - this.spotMinX) * zoom + (srcSpot.name.length * 12 + 20) / 2
          var dy = (spot.y - this.spotMinY) * zoom + 18, sy = (srcSpot.y - this.spotMinY) * zoom + 18
          var x = dx - sx
          var y = dy - sy
          var angle = 0;
          if (x < 0 && y < 0)
            angle = 180 * Math.atan2(-y, -x) / Math.PI
          else if (x > 0 && y > 0)
            angle = 180 * Math.atan2(x, y) / Math.PI * -1 - 90
          else if (x < 0 && y > 0)
            angle = 180 * Math.atan2(y, -x) / Math.PI * -1
          else
            angle = 180 * Math.atan2(x, -y) / Math.PI + 90
          var width = 'width:' + Math.sqrt(Math.pow(x, 2) + Math.pow(y, 2)) + 'px;'
          var left = 'left:' + dx + 'px;'
          var top = 'top:' + dy + 'px;'
          var rotate = 'transform:rotate(' + angle + 'deg);-webkit-transform:rotate(' + angle + 'deg);;';
          return 'display:block;' + width + left + top + rotate
        }
        return ''
      },
      spotList(war) {
        this.showWar = false
        this.showSpot = true
        this.showQuest = false
        this.spots = war.spots
        var x = 999999, y = 999999
        war.spots.forEach(spot => {
          x = parseInt(spot.x) < x ? parseInt(spot.x) : x
          y = parseInt(spot.y) < y ? parseInt(spot.y) : y
        })
        if (this.spots[this.spots.length - 1].x == '999999')
          this.spots.pop()
        this.spotMinX = x - 20
        this.spotMinY = y - 20
        if (war.spots.length == 1)
          this.questList(war.spots[0])
      },
      clone(obj) {
        var nobj = {}
        for (var attr in obj) {
          if (obj.hasOwnProperty(attr)) {
            if (typeof (obj[attr]) !== "function" && typeof (obj[attr]) !== "object") {
              if (obj[attr] === null) {
                obj[attr] = null;
              }
              else {
                nobj[attr] = obj[attr];
              }
            }
          }
        }
        return nobj
      },
      setQuestList(res) {
        this.$parent.loading = false
        if (!res.data || res.data.code != 0)
          return
        var userQuests = res.data.data
        var wars = this.$store.state.mapData.wars
        var nwars = []
        wars.forEach(w => {
          var spots = []
          w.spots.forEach(s => {
            var quests = []
            s.quests.forEach(q => {
              var uq = userQuests.find(q2 => q2.id == q.id)
              if (uq) {
                if (!nwars.find(w2 => w2.id == w.id))
                  nwars.push(this.clone(w))
                if (!spots.find(s2 => s2.id == s.id))
                  spots.push(this.clone(s))
                q.phase = uq.phase
                quests.push(q)
              }
            })
            var spot = spots.find(ns => ns.id == s.id)
            if (spot)
              spot.quests = quests
          })
          var war = nwars.find(nw => nw.id == w.id)
          if (war) {
            war.spots = spots
            war.spotRoads = w.spotRoads
          }
        })
        this.data.wars = nwars
        this.nextQuest()
      },
      loadQuestList(id) {
        this.$store.dispatch('FETCH_ROLE_QUEST', id).then(res => {
          this.setQuestList(res)
        })
      }
    },
    components: {},
    computed: {
      loading() {
        return this.$store.state.loading
      },
      roleCount() {
        return this.$store.state.roles.length
      },
      taskCount() {
        return this.$store.state.tasks.length
      }
    },
    beforeDestroy() {
      window.removeEventListener('click', this.fire)
    },
    beforeMount() {
      this.fire = (e) => { if (this.showSpot && !this.showQuest) this.zoom += e.wheelDelta / 4000 }
      window.addEventListener('mousewheel', this.fire)
      this.$parent.loading = true
      this.$parent.showErrLoding = true
      this.$parent.showBody = false
      this.$store.dispatch('LOAD_MAP_DATA').then(() => {
        if (this.$route.params.id) {
          this.loadQuestList(this.$route.params.id)
        } else {
          this.$parent.loading = false
          this.data = this.$store.state.mapData
        }
      })
    }
  }
</script>

<style>
.map-warp{
  color:#6d757a
}
  .war-list{
    width: 680px;
    margin: 0 auto;
  }
  .war-list .war-item{
    border-radius: 4px;
    background-color: #fff;
    margin-bottom: 20px;
    padding: 30px;
    text-align: center;
    cursor: pointer;
  }
  .war-list .war-item:hover{
    color: #00a1d6!important
  }
  .war-list .war-item .war-header{
    height: 20px;
    font-size: 14px
  }
  .war-list .war-item .long-name{
    font-size: 24px;
    text-align: center;
  }

  .spot-list{
        position: relative;
  }
  .spot-list .back{
    position: fixed;
    left: 245px;
    top: 20px;
    font-size: 14px;
    border: 1px solid;
    padding: 4px 12px;
    border-radius: 5px;
    background: #fff;
  }
  .spot-list .spot-item{
    word-break: keep-all;
    white-space: normal;
        position: absolute;
        background: #fff;
        padding: 10px;
        border-radius: 5px;
        box-shadow: 1px 1px 2px #dfe5ed;
        z-index: 1;
        cursor: pointer
  }
  .spot-list .spot-road{
        display: none;
        position: absolute;
        background: #bed0d6;
        height: 4px;
        transform-origin: 0;
        z-index: 0;
        line-height: 4px;
        text-align: center
  }
  .spot-list .spot-road .lt{
    margin-top: -4px;
    border-color: #5db8fb;
  }
  .spot-list .spot-item:hover
  { 
    color: #00a1d6!important;
    box-shadow: 0 0 8px rgba(0, 161, 214, 0.55);
  }
  .spot-list .spot-item.current
  { 
    background: #f5f5dc;
  }
  .quest-list{
    width: 350px;
    position: fixed;
    top: 0;
    right: 0;
    z-index: 99;
    background: #fff;
    height: 100%;
    border-left: 1px solid #e5e9ef;
  }
  .quest-list .quest-item{
        padding: 20px;
    border-bottom: 1px solid #ecf1f5;
    cursor: pointer
  }
  .quest-list .quest-item:hover{
    color: #00a1d6!important;
    background: #f1f3f7;
  }
  .quest-list .quest-item span{
    background: rgb(59,167,188);border-radius: 3px;padding: 2px; color: #fff;font-family: monospace;float: right;margin-left: 5px;
  }
  .quest-list .quest-item .quest-head{
        margin-bottom: 10px;
  }
  .quest-warp{ 
    overflow: auto;
    height: 90%;
  }
  .quest-phase.gt.clear {
    border-color: #37c8f7;
  }
  .quest-item.current {
    background: #f5f5dc;
  }
  .gt,.lt{
    border: 2px solid;
    height: 10px;
    width: 10px;
    transform: rotate(45deg);
    -webkit-transform: rotate(45deg);
    display: inline-block;
  }
  .lt{
    border-top: 0;
    border-right: 0
  }
  .gt{
    border-left: 0;
    border-bottom: 0;
  }
</style>