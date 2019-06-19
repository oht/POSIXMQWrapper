#define _POSIX_C_SOURCE 200809L
#include <errno.h>
#include <mqueue.h>
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

typedef void *NotifyHandler;

mqd_t pmq_open(const char *name, int oflag, int *errnum) {
  mqd_t ret;
  ret = mq_open(name, oflag);

  if (ret == -1) {
    if (errnum != NULL)
      *errnum = errno;
  }
  return ret;
}

mqd_t pmq_open_attr(const char *name, int oflag, int mode, int maxMsg,
                    int msgSize, int *errnum) {
  mqd_t ret;
  struct mq_attr attr;
  /* initialize the queue attributes */
  attr.mq_flags = 0;
  attr.mq_maxmsg = maxMsg;
  attr.mq_msgsize = msgSize;
  attr.mq_curmsgs = 0;
  ret = mq_open(name, oflag, mode, &attr);

  if (ret == -1) {
    if (errnum != NULL)
      *errnum = errno;
  }
  return ret;
}

mqd_t pmq_open_defaults(const char *name, int oflag, int mode, int *errnum) {
  mqd_t ret;
  void *attr;
  ret = mq_open(name, oflag, mode, &attr);

  if (ret == -1) {
    if (errnum != NULL)
      *errnum = errno;
  }
  return ret;
}

mqd_t pmq_close(mqd_t mqdes, int *errnum) {
  mqd_t ret;
  ret = mq_close(mqdes);

  if (ret == -1) {
    if (errnum != NULL)
      *errnum = errno;
  }
  return ret;
}

// Expose sigval instead of comples sigevent structure
mqd_t pmq_notify(mqd_t mqdes, NotifyHandler notifyHandler, int *errnum) {
  int ret;
  struct sigevent sev;
  // Notify via thread
  sev.sigev_notify = SIGEV_THREAD;
  sev.sigev_notify_function = notifyHandler;
  // Could be pointer to pthread_attr_t structure
  sev.sigev_notify_attributes = NULL;
  sev.sigev_value.sival_int = mqdes;
  ret = mq_notify(mqdes, &sev);

  if (ret == -1) {
    if (errnum != NULL)
      *errnum = errno;
  }
  return ret;
}

// Expose timeout instead of timespec structure.
mqd_t pmq_receive(mqd_t mqdes, char *msg_ptr, size_t msg_len,
                  unsigned int *msg_prio, int timeout, int *errnum) {
  int ret;
  struct timespec tm;
  char buf[msg_len];

  if (timeout == 0) {
    ret = mq_receive(mqdes, msg_ptr, msg_len, msg_prio);
  } else {
    clock_gettime(CLOCK_REALTIME, &tm);
    tm.tv_sec += timeout; // Set for 20 seconds
    ret = mq_timedreceive(mqdes, buf, msg_len, msg_prio, &tm);
    memcpy(msg_ptr, buf, msg_len);
  }

  if (ret == -1) {
    if (errnum != NULL)
      *errnum = errno;
  }
  return ret;
}

mqd_t pmq_send(mqd_t mqdes, const char *msg_ptr, size_t msg_len,
               unsigned int msg_prio, int timeout, int *errnum) {
  int ret;
  struct timespec tm;

  if (timeout == 0) {
    ret = mq_send(mqdes, msg_ptr, msg_len, msg_prio);
  } else {
    clock_gettime(CLOCK_REALTIME, &tm);
    tm.tv_sec += timeout; // Set for 20 seconds
    ret = mq_timedsend(mqdes, msg_ptr, msg_len, msg_prio, &tm);
  }

  if (ret == -1) {
    if (errnum != NULL)
      *errnum = errno;
  }
  return ret;
}

mqd_t pmq_unlink(const char *name, int *errnum) {
  mqd_t ret;
  ret = mq_unlink(name);
  if (ret == -1) {
    if (errnum != NULL)
      *errnum = errno;
  }
  return ret;
}
