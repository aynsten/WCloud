<template>
  <span>
    <span v-if="moment_time!==null">
      <span v-if="recent">
        <a-tooltip>
          <template slot="title">{{format_time}}</template>
          <span>{{relative_time}}</span>
        </a-tooltip>
      </span>
      <span v-else :title="format_time">{{format_time}}</span>
    </span>
  </span>
</template>

<script>
import moment from 'moment'

export default {
  props: {
    time_str: {
      type: String,
      default: null
    },
    time_format: {
      type: String,
      default: 'YYYY-MM-DD HH:mm:ss'
    }
  },
  data() {
    return {
      moment_time: null
    }
  },
  created() {
    this.parse(this.time_str)
  },
  methods: {
    parse(time_data) {
      if (time_data) {
        try {
          this.moment_time = moment.utc(time_data)
        } catch (e) {
          console.log(e)
        }
      } else {
        this.moment_time = null
      }
    }
  },
  computed: {
    recent() {
      return this.moment_time && Math.abs((this.moment_time - moment.utc()) / 1000 / 60 / 60) < 12
    },
    format_time() {
      if (this.moment_time) {
        return moment
          .utc(this.moment_time)
          .add(8, 'hour')
          .format(this.time_format)
      }
    },
    relative_time() {
      if (this.moment_time) {
        return this.moment_time.from(moment.utc())
      }
    }
  },
  watch: {
    time_str(val) {
      this.parse(val)
    }
  }
}
</script>
